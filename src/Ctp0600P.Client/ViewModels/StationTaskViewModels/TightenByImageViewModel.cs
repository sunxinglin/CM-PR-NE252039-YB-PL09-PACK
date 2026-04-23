using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using AsZero.Core.Services.Messages;

using Ctp0600P.Client.Apis;
using Ctp0600P.Client.PLC.Context;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.Protocols.BoltGun.Models;
using Ctp0600P.Client.Views.Windows;
using Ctp0600P.Shared;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Reactive.Bindings;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DBEntity.Production;
using Yee.Entitys.DTOS;
using Yee.Entitys.DTOS.StationTaskDataDTOS;
using Yee.Entitys.Production;

namespace Ctp0600P.Client.ViewModels.StationTaskViewModels
{
    public class TightenByImageViewModel : TaskViewModelBase
    {
        private readonly Base_StationTask_TightenByImage _taskTightenByImage;
        private readonly APIHelper _apiHepler;
        private readonly IMediator mediator;
        private readonly IServiceProvider _service;
        private readonly StationPLCContext _stationPLCContext;

        private ScrewLayoutInfo CurrentWorkScrew => this.ScrewLayout.Value.FirstOrDefault(f => f.Status.Value != "OK");

        public TightenByImageViewModel(APIHelper apiHepler, IMediator mediator, IServiceProvider service, StationTaskDTO stationTaskDTO, StationPLCContext stationPLCContext, IOptionsMonitor<StepStationSetting> stepStationSettings)
        {
            _taskTightenByImage = stationTaskDTO.Base_StationTask_TightenByImage;
            _apiHepler = apiHepler;
            this.mediator = mediator;
            _service = service;
            _StationTaskDTO = stationTaskDTO;
            _stationPLCContext = stationPLCContext;

            this.PageScale = new ReactiveProperty<double> { Value = 1.0 };
            stepStationSettings.OnChange(settings =>
            {
                PageScale.Value = settings.TightenByImagePageScale > 0 ? settings.TightenByImagePageScale : 1.0;
            });
            PageScale.Value = stepStationSettings.CurrentValue.TightenByImagePageScale > 0 ? stepStationSettings.CurrentValue.TightenByImagePageScale : 1.0;

            Canva_Width = new ReactiveProperty<float> { Value = _taskTightenByImage?.Layout?.CanvasWidth ?? 0 };
            Canva_Height = new ReactiveProperty<float> { Value = _taskTightenByImage?.Layout?.CanvasHeight ?? 0 };
            this.ScrewNo = new ReactiveProperty<int?>();
            this.Torque = new ReactiveProperty<string>();
            this.Angle = new ReactiveProperty<string>();
            this.CurProgram = new ReactiveProperty<int> { Value = _taskTightenByImage.ProgramNo };
            using var scope = service.CreateScope();
            var sp = scope.ServiceProvider;
            var options = sp.GetRequiredService<IOptions<ApiServerSetting>>();
            var uri = options.Value.BaseUrl;
            var resourceImage = new BitmapImage();
            resourceImage.BeginInit();
            resourceImage.UriSource = new Uri(uri + _taskTightenByImage.ImageUrl); // 项目资源路径
            resourceImage.EndInit();
            if (_taskTightenByImage.Layout == null)
            {
                mediator.Publish(new AlarmSYSNotification
                {
                    Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.SERVER_MODULE,
                    Description = $"未配置图示拧紧布图!"
                });
                return;
            }

            if (_taskTightenByImage.Layout.Points.Count != _taskTightenByImage.ScrewNum)
            {
                mediator.Publish(new AlarmSYSNotification
                {
                    Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.SERVER_MODULE,
                    Description = $"图示拧紧布局点位数量与配方数量不匹配!"
                });
                return;
            }
            ImageSource = new ReactiveProperty<ImageSource> { Value = resourceImage };
            ScrewLayout = new ReactiveProperty<IList<ScrewLayoutInfo>> { Value = _taskTightenByImage.Layout.Points.OrderBy(o => o.OrderNo).Select(s => new ScrewLayoutInfo(s.OrderNo, new Thickness(s.Point_X, s.Point_Y, 0, 0), "Undo")).ToList() };
            BindHistoryData();

            this.ChangeReverse = new ReactiveCommand().WithSubscribe(async () =>
            {
                // this.ChangeProgramByPowerCheck();
            });
        }

        private void BindHistoryData()
        {
            var records = App.HisTaskData2?.StationTaskRecords;
            if (records == null || _taskTightenByImage == null)
            {
                return;
            }

            var hisRecord = records.FirstOrDefault(f => f.StationTaskId == _taskTightenByImage.StationTaskId);
            if (hisRecord?.TightenByImages == null || hisRecord.TightenByImages.Count == 0)
            {
                return;
            }
            // 同一序号可能存在多条历史（多次保存/NG/反拧），按序号取最新一条用于界面状态展示
            var listRecord = hisRecord.TightenByImages
                .Where(w => w.OrderNo.HasValue)
                .OrderByDescending(o => o.CreateTime)
                .ThenByDescending(o => o.Id)
                .ToList();

            var latestMap = listRecord
                .Where(w => (w.OrderNo ?? 0) > 0)
                .GroupBy(g => g.OrderNo!.Value)
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var item in ScrewLayout.Value)
            {
                if (!latestMap.TryGetValue(item.OrderNo, out var record) || record == null)
                {
                    continue;
                }

                item.Status.Value = record.ResultIsOK ? "OK" : "NG";
            }
        }

        public void FindNextUndo(int? preferOrderNo = null)
        {
            if (ScrewLayout?.Value == null || ScrewLayout.Value.Count == 0)
            {
                return;
            }


            foreach (ScrewLayoutInfo s in ScrewLayout.Value)
            {
                if (s.Status.Value == "Doing" || s.Status.Value == "Wait")
                {
                    s.Status.Value = "Undo";
                }
            }

            var notOk = ScrewLayout.Value.Where(w => w.Status.Value != "OK").ToList();
            if (notOk.Count == 0)
            {
                CompleteTask();
                return;
            }

            ScrewLayoutInfo? first;
            if (preferOrderNo.HasValue)
            {
                first = notOk.FirstOrDefault(f => f.OrderNo == preferOrderNo.Value) ?? notOk.FirstOrDefault();
            }
            else
            {
                first = notOk.FirstOrDefault();
            }

            if (first == null)
            {
                return;
            }

            var second = notOk.FirstOrDefault(f => f.OrderNo != first.OrderNo);

            first.Status.Value = "Doing";
            if (second != null)
            {
                second.Status.Value = "Wait";
            }

            this.ScrewNo.Value = first.OrderNo;

            // 由 MES 主动通过 PLC 发起当前序号的拧紧请求
            _ = RequestTightenForOrderAsync(first.OrderNo, IsNeedReverse);
        }



        #region 【拧紧枪 使能 去使能】

        // 保留接口以便后续需要时通过 PLC 请求扩展。
        public void EnableBoltGuns()
        {
            // 由 PLC 触发拧紧开始。
        }

        public void DisableBoltGun(string devicesNo)
        {
            // 由 PLC 控制。
        }

        public void DisableBoltGuns()
        {
            // 由 PLC控制。
        }
        public ReactiveProperty<int> CurProgram { get; set; }
        private bool IsNeedReverse { get; set; }

        public void ReverseScrew()
        {
            // 正反转切换只更新当前期望程序号，由 PLC 按程序号执行。
            var targetProgram = this.IsNeedReverse && App._StepStationSetting.ReversePset != 0
                ? App._StepStationSetting.ReversePset
                : (int)_taskTightenByImage.ProgramNo;
            CurProgram.Value = targetProgram;

            // 复位拧紧 NG 后，按当前序号重新发起一次拧紧/反拧请求
            if (CurrentWorkScrew != null)
            {
                _ = RequestTightenForOrderAsync(CurrentWorkScrew.OrderNo, IsNeedReverse);
            }
        }

        public void ReverseScrew(Base_StationTask_TightenByImage screw)
        {
            var targetProgram = this.CurProgram.Value != App._StepStationSetting.ReversePset
                ? App._StepStationSetting.ReversePset
                : (int)screw.ProgramNo;
            this.CurProgram.Value = targetProgram;
        }

        /// <summary>
        /// 通过 PLC 请求当前序号的拧紧/反拧
        /// </summary>
        private async Task RequestTightenForOrderAsync(int orderNo, bool isReverse)
        {
            try
            {
                if (orderNo <= 0 || _taskTightenByImage == null)
                {
                    return;
                }

                // 当前任务关联的拧紧枪设备号
                var deviceNosCfg = (_taskTightenByImage.DevicesNos ?? string.Empty)
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (deviceNosCfg.Length == 0)
                {
                    return;
                }

                // 加载工位配置中的拧紧枪资源
                var resources = await _apiHepler.LoadStationProResourceConfig(ProResourceTypeEnum.拧紧枪);
                if (resources == null || resources.Count == 0)
                {
                    return;
                }

                // 确定当前要用的程序号（正拧/反拧）
                var programNo = CurProgram != null && CurProgram.Value > 0
                    ? CurProgram.Value
                    : (int)_taskTightenByImage.ProgramNo;

                foreach (var res in resources)
                {
                    if (string.IsNullOrWhiteSpace(res.DeviceNo))
                    {
                        continue;
                    }

                    // 只给当前图示拧紧任务配置的设备发请求
                    if (!deviceNosCfg.Contains(res.DeviceNo))
                    {
                        continue;
                    }

                    if (!ushort.TryParse(res.DeviceNo, out var devNo))
                    {
                        continue;
                    }

                    var brand = (ushort)(res.DeviceBrand == DeviceBrand.马头 ? 1 : 2);

                    _stationPLCContext.StationTightenStartReqs.Add(new StationPLCContext.StationTightenStartReq
                    {
                        DeviceNo = devNo,
                        DeviceBrand = brand,
                        ProgramNo = (ushort)programNo,
                        // 图示拧紧所在工位只有这一类拧紧任务，序号直接用螺丝序号
                        SortNo = (ushort)orderNo,
                        ScrewCountBeforeCurrentTask = 0,
                        Resource = $"[{DateTimeOffset.Now.UtcTicks}]TightenByImage OrderNo={orderNo}, Reverse={isReverse}"
                    });
                }
            }
            catch (Exception ex)
            {
                await mediator.Publish(new AlarmSYSNotification
                {
                    Code = AlarmCode.系统运行错误,
                    Name = AlarmCode.系统运行错误.ToString(),
                    Module = AlarmModule.DESOUTTER_MODULE,
                    Description = $"图示拧紧下发PLC拧紧请求失败：{ex.Message}"
                });
            }
        }
        public async void ChangeProgramByPowerCheck()
        {
            if (App._StepStationSetting.ReversePset == 0)
            {
                return;
            }

            using (var scope = _service.CreateScope())
            {
                var sp = scope.ServiceProvider;

                var wincheck = sp.GetRequiredService<CheckPowerPage>();
                ((CheckPowerViewModel)wincheck.DataContext).Action = async delegate
                {
                    wincheck.Close();
                    this.ReverseScrew(_taskTightenByImage);
                    IsNeedReverse = !IsNeedReverse;
                };
                ((CheckPowerViewModel)wincheck.DataContext).ModuleName = "正反转切换";
                //((CheckPowerViewModel)wincheck.DataContext).ResetType = ResetType.Normal;
                wincheck.ShowDialog();
            }
        }
        #endregion

        #region 【处理拧紧枪消息】

        public async void CatchBoltGunMessage(BoltGunRequest request)
        {
            try
            {
                var BoltNotDead = await App.CompareAndSavelastTighteningIDFromJsonFile(Convert.ToInt32(request.DeviceNo), request.TightenID, request.DeviceNo);
                if (!BoltNotDead && !App._StepStationSetting.IsDebug)
                {
                    // this.DisableBoltGun(request.DeviceNo);
                    await mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.拧紧枪错误, Name = AlarmCode.拧紧枪错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"拧紧枪{request.DeviceNo}发送的拧紧ID与上次的拧紧ID相同，请确认拧紧枪是否故障" });
                    return;
                }

                var sysResult = DealBoltResult(request);
                await mediator.Publish(new UILogNotification
                {
                    LogMessage = new LogMessage
                    {
                        Level = LogLevel.Information,
                        Timestamp = DateTime.Now,
                        Content =
                            $"[图示拧紧判定] ResultIsOK={request.ResultIsOK}, Pset={request.ProgramNo}, " +
                            $"Torque={request.FinalTorque}, Angle={request.FinalAngle}; " +
                            $"Cfg: MinT={_taskTightenByImage.MinTorque}, MaxT={_taskTightenByImage.MaxTorque}, " +
                            $"MinA={_taskTightenByImage.MinAngle}, MaxA={_taskTightenByImage.MaxAngle}, Float={_taskTightenByImage.FloatFactor}; " +
                            $"sysResult={sysResult}"
                    }
                });

                this.Torque.Value = request.FinalTorque + "N";
                this.Angle.Value = request.FinalAngle + "°";


                if (request.ProgramNo == App._StepStationSetting.ReversePset)//处理反转
                {
                    await mediator.Publish(new UILogNotification { LogMessage = new LogMessage { Level = LogLevel.Information, Content = $"收到拧紧枪{request.DeviceNo}的反拧结果" } });

                    if (request.ResultIsOK)
                    {
                        //保存返工数据
                        await mediator.Publish(new UILogNotification { LogMessage = new LogMessage { Level = LogLevel.Information, Content = $"收到拧紧枪{request.DeviceNo}的反拧结果OK" } });

                        if (!string.IsNullOrWhiteSpace(_taskTightenByImage.ReverseGroup))
                        {
                            await App.UploadReverseSingleAsync(_taskTightenByImage.ReverseGroup, _taskTightenByImage.UpMesCode, CurrentWorkScrew.OrderNo, request.FinalTorque, request.FinalAngle);
                        }
                        var dto = new ScrewDataDTO
                        {
                            StationTaskId = _StationTaskDTO.StationTaskId,
                            NgTimes = App.GetNgTimes(Convert.ToInt32(request.DeviceNo)),
                            MainId = App.HisTaskData2.MainId,
                            StationCode = App.CurrentStationCode,
                            FinalTorque = request.FinalTorque,
                            FinalAngle = request.FinalAngle,
                            UploadMesCode = _taskTightenByImage.UpMesCode,
                            OrderNo = CurrentWorkScrew.OrderNo,
                            TaskName = _taskTightenByImage.TaskName
                        };
                        SaveReverseData(dto);//保存失败不响应

                        IsNeedReverse = false;
                        CurProgram.Value = _taskTightenByImage.ProgramNo;
                    }
                    return;
                }

                await mediator.Publish(new UILogNotification { LogMessage = new LogMessage { Level = LogLevel.Information, Content = $"收到拧紧枪{request.DeviceNo}的拧紧结果: 螺丝规格：{_taskTightenByImage.TaskName}; 序号：{CurrentWorkScrew.OrderNo} ; 扭力：{request.FinalTorque}N; 角度：{request.FinalAngle}°" } });
                if (!DealBoltNgs(request))
                {
                    // this.DisableBoltGun(request.DeviceNo);
                    return;
                }


                if (request.ResultIsOK && sysResult)
                {
                    var dto = new ScrewDataDTO
                    {
                        MainId = App.HisTaskData2.MainId,
                        TaskName = _taskTightenByImage.TaskName,
                        FinalAngle = request.FinalAngle,
                        FinalTorque = request.FinalTorque,
                        OrderNo = CurrentWorkScrew.OrderNo,
                        StationCode = App.CurrentStationCode,
                        StationTaskId = _taskTightenByImage.StationTaskId,
                        UploadMesCode = _taskTightenByImage.UpMesCode
                    };
                    var saveOk = await SaveScrewData(dto);
                    if (saveOk)
                    {

                        CurrentWorkScrew.Status.Value = "OK";
                        FindNextUndo();
                        App.ClearNgTimes(Convert.ToInt32(request.DeviceNo));
                    }
                    else
                    {
                        await mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"拧紧数据保存失败，请联系管理员！" });
                    }
                }
                else
                {
                    CurrentWorkScrew.Status.Value = "NG";
                    // this.DisableBoltGun(request.DeviceNo);

                    App.AddNgTimes(Convert.ToInt32(request.DeviceNo));
                    var overflow = App.CheckOverflow(Convert.ToInt32(request.DeviceNo));
                    if (overflow)
                    {
                        if(!string.IsNullOrWhiteSpace(_taskTightenByImage.NcCode) )
                        {
                            await mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.多次拧紧NG, Name = AlarmCode.多次拧紧NG.ToString(), Module = "多次拧紧NG", Description = $"拧紧枪{request.DeviceNo}出现连续{App._StepStationSetting.NGUpTimes}次NG，即将自动NC" });
                            await App._RealtimePage.TryNc(_taskTightenByImage.NcCode);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(_taskTightenByImage.NgGroup))
                    {
                        await App.UploadNgSingleAsync(_taskTightenByImage.NgGroup, _taskTightenByImage.UpMesCode, CurrentWorkScrew.OrderNo, request.FinalTorque, request.FinalAngle);
                    }

                    var dto = new ScrewDataDTO
                    {
                        StationTaskId = _StationTaskDTO.StationTaskId,
                        NgTimes = App.GetNgTimes(Convert.ToInt32(request.DeviceNo)),
                        MainId = App.HisTaskData2.MainId,
                        StationCode = App.CurrentStationCode,
                        FinalTorque = request.FinalTorque,
                        FinalAngle = request.FinalAngle,
                        UploadMesCode = _taskTightenByImage.UpMesCode,
                        OrderNo = CurrentWorkScrew.OrderNo,
                        TaskName = _taskTightenByImage.TaskName
                    };
                    SaveNgData(dto);//保存失败不响应

                    if (!request.ResultIsOK)
                    {
                        await mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.拧紧枪错误, DeviceNo = request.DeviceNo, Name = AlarmCode.拧紧NG.ToString(), Module = "拧紧NG", PackCode = App.PackBarCode, Description = $"拧紧NG，请复位后重拧,设备{request.DeviceNo}已连续NG{App.GetNgTimes(Convert.ToInt32(request.DeviceNo))}次！\r\n任务:{_taskTightenByImage.TaskName}\r\n程序号:{_taskTightenByImage.ProgramNo}" });
                        IsNeedReverse = true;
                        return;
                    }

                    if (!sysResult)
                    {
                        await mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.拧紧枪错误, DeviceNo = request.DeviceNo, Name = AlarmCode.拧紧NG.ToString(), Module = "拧紧NG", PackCode = App.PackBarCode, Description = $"系统防呆NG，请复位后重拧,设备{request.DeviceNo}已连续NG{App.GetNgTimes(Convert.ToInt32(request.DeviceNo))}次！\r\n任务:{_taskTightenByImage.TaskName}\r\n程序号:{_taskTightenByImage.ProgramNo}" });
                        IsNeedReverse = true;
                    }

                }

            }
            catch (Exception ex)
            {
                // this.DisableBoltGun(request.DeviceNo);
                await mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"{ex.Message}/{ex.StackTrace}" });
            }
        }


        private bool DealBoltNgs(BoltGunRequest request)
        {

            if (request.FinalTorque < Convert.ToDecimal(0.4))
            {
                mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.空转NG, Name = AlarmCode.空转NG.ToString(), Module = "空转NG", Description = $"{request.DeviceNo}号枪空转NG" });
                return false;
            }

            if (_taskTightenByImage.MaxTorque == null || _taskTightenByImage.MinTorque == null || _taskTightenByImage.MinAngle == null)
            {
                return true;
            }

            var torquehalf = (_taskTightenByImage.MaxTorque + _taskTightenByImage.MinTorque) / 2 * (decimal)_taskTightenByImage.FloatFactor < request.FinalTorque;
            var anglehalf = _taskTightenByImage.MinAngle < request.FinalAngle;

            if (!torquehalf && !anglehalf)
            {
                mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.防重打NG, Name = AlarmCode.防重打NG.ToString(), Module = "防重打NG", Description = $"{request.DeviceNo}号枪防重打NG" });
                return false;
            }
            return true;
        }


        private bool DealBoltResult(BoltGunRequest request)
        {
            var torqueOk = _taskTightenByImage.MinTorque == null || _taskTightenByImage.MaxTorque == null ? true : CompareArea(request.FinalTorque, _taskTightenByImage.MinTorque, _taskTightenByImage.MaxTorque);
            var AngleOk = _taskTightenByImage.MinAngle == null || _taskTightenByImage.MaxAngle == null ? true : CompareArea(request.FinalAngle, _taskTightenByImage.MinAngle, _taskTightenByImage.MaxAngle);
            return torqueOk && AngleOk;
        }

        private bool CompareArea(decimal value, decimal? min, decimal? max)
        {
            if (min == null || max == null || min >= max)
                return true;
            return value > min && value < max;
        }


        /// <summary>
        /// 当前任务完成
        /// </summary>
        private async void CompleteTask()
        {
            _StationTaskDTO.HasFinish = true;
            // DisableBoltGuns();
            OnCompleteTask(_StationTaskDTO);
        }

        public async Task<bool> SaveScrewData(ScrewDataDTO dto)
        {
            return await App._RealtimePage.SaveTightenByImageData(dto);
        }

        public async Task<bool> SaveReverseData(ScrewDataDTO dto)
        {
            return await App._RealtimePage.SaveReverseData(dto);
        }

        public async Task<bool> SaveNgData(ScrewDataDTO dto)
        {
            return await App._RealtimePage.SaveNgData(dto);
        }

        public async Task<bool> SetScrewTaskFinish(Base_StationTaskScrew screw)
        {
            screw.CreateUserID = App._RealtimePage.WorkingUserId;
            screw.CreateTime = DateTime.Now;
            return await App._RealtimePage.SetScrewTaskFinish(screw);
        }

        #endregion



        public StationTaskDTO _StationTaskDTO { get; }

        public ReactiveProperty<float> Canva_Width { get; }
        public ReactiveProperty<float> Canva_Height { get; }

        public ReactiveProperty<ImageSource> ImageSource { get; }

        public ReactiveProperty<IList<ScrewLayoutInfo>> ScrewLayout { get; }

        public ReactiveProperty<string> Torque { get;  }
        public ReactiveProperty<string> Angle { get;  }
        public ReactiveProperty<int?> ScrewNo { get;  }
        public ReactiveCommand ChangeReverse { get; }
        public ReactiveProperty<double> PageScale { get; set; }
        public class ScrewLayoutInfo
        {
            public ScrewLayoutInfo(int orderNo, Thickness marginLayout, string status)
            {
                OrderNo = orderNo;
                MarginLayout = marginLayout;
                Status = new ReactiveProperty<string>() { Value = status };
            }
            public int OrderNo { get; set; }

            public Thickness MarginLayout { get; set; }

            public ReactiveProperty<string> Status { get; set; }
        }
    }
}
