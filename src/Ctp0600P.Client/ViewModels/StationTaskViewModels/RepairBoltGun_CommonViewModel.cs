using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

using Ctp0600P.Client.Apis;
using Ctp0600P.Client.CommonEntity;
using Ctp0600P.Client.DTOS;
using Ctp0600P.Client.PLC.Context;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.Protocols.BoltGun.Models;
using Ctp0600P.Shared;

using MediatR;

using Microsoft.Extensions.Options;

using Reactive.Bindings;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DBEntity.Production;
using Yee.Entitys.DBEntity.ProductionRecords;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;

namespace Ctp0600P.Client.ViewModels.StationTaskViewModels
{
    public class RepairBoltGun_CommonViewModel : TaskViewModelBase
    {
        private readonly Base_StationTask_TightenRework _taskReworkScrewBase;
        private readonly APIHelper _apiHelper;
        private readonly IMediator _mediator;
        private readonly StationPLCContext _stationPLCContext;
        private readonly IOptionsMonitor<StepStationSetting> stepStationSettings;
        private readonly IServiceProvider _serviceProvider;

        public Proc_AutoBoltInfo_Detail_DTO CurDoingScrew { get; set; } = null;

        public RepairBoltGun_CommonViewModel(StationTaskDTO stationTaskDTO, IServiceProvider serviceProvider, APIHelper apiHelper, IMediator mediator, StationPLCContext stationPLCContext, IOptionsMonitor<StepStationSetting> stepStationSettings)
        {
            _serviceProvider = serviceProvider;
            this._apiHelper = apiHelper;
            this._mediator = mediator;
            _StationTaskDTO = stationTaskDTO;
            _taskReworkScrewBase = stationTaskDTO.Base_StationTask_TightenRework;

            this.GridX = new ReactiveProperty<double> { Value = 0 };
            this.GridY = new ReactiveProperty<double> { Value = 0 };
            this.ImageSource = new ReactiveProperty<BitmapImage> { Value = null };
            this.ScrewListObserver = new ReactiveCollection<ScrewLocation>();
            InitData(_taskReworkScrewBase.ScrewNum);
            this.Torque = new ReactiveProperty<string> { Value = "0N" };
            this.Angle = new ReactiveProperty<string> { Value = "0°" };
            this.CurrentScrewNo = new ReactiveProperty<int> { Value = 0 };
            this.FlipX = new ReactiveProperty<double> { Value = 1 };
            this.FlipY = new ReactiveProperty<double> { Value = 1 };
            this.PageScale = new ReactiveProperty<double> { Value = 1.0 };

            BindHistory();
            if (!_StationTaskDTO.HasFinish)
            {
                _ = LoadScrewList();
            }

            _stationPLCContext = stationPLCContext;
            this.stepStationSettings = stepStationSettings;

            stepStationSettings.OnChange(settings =>
            {
                FlipX.Value = settings.RepairBoltFlipX ? -1 : 1;
                FlipY.Value = settings.RepairBoltFlipY ? -1 : 1;
                PageScale.Value = settings.RepairPageScale > 0 ? settings.RepairPageScale : 1.0;
            });
            FlipX.Value = stepStationSettings.CurrentValue.RepairBoltFlipX ? -1 : 1;
            FlipY.Value = stepStationSettings.CurrentValue.RepairBoltFlipY ? -1 : 1;
            PageScale.Value = stepStationSettings.CurrentValue.RepairPageScale > 0 ? stepStationSettings.CurrentValue.RepairPageScale : 1.0;
        }

        /// <summary>
        /// 加载未OK的数据
        /// </summary>
        /// <returns></returns>
        public async Task LoadScrewList()
        {
            var result = await _apiHelper.LoadAutoTightenData(App.PackBarCode, _taskReworkScrewBase.ReworkType, _taskReworkScrewBase.ScrewNum);
            if (result.Code != 200)
            {
                await _mediator.Publish(new AlarmSYSNotification
                {
                    Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.SERVER_MODULE,
                    Description = $"{result.Message}"
                });
                return;
            }
            var ScrewList = result.Result.OrderBy(o => o.OrderNo).Select(s => new Proc_AutoBoltInfo_Detail_DTO_Ext
            {
                ResultIsOK = s.ResultIsOK,
                TaskName = _taskReworkScrewBase.TaskName,
                AngleStatus = s.AngleStatus,
                Angle_Max = s.Angle_Max,
                Angle_Min = s.Angle_Min,
                Constant1 = s.Constant1,
                Constant2 = s.Constant2,
                FinalAngle = s.FinalAngle,
                FinalTorque = s.FinalTorque,
                OrderNo = s.OrderNo,
                ProgramNo = s.ProgramNo,
                TargetAngle = s.TargetAngle,
                TargetTorqueRate = s.TargetTorqueRate,
                TorqueRate_Max = s.TorqueRate_Max,
                TorqueRate_Min = s.TorqueRate_Min,
                TorqueStatus = s.TorqueStatus,
                Status = s.ResultIsOK ? "OK" : "NG"
            }).ToList();
            AutoScrewDataList = new ObservableCollection<Proc_AutoBoltInfo_Detail_DTO_Ext>(ScrewList);
            var HasRepairData = App.HisTaskData2.StationTaskRecords.FirstOrDefault(f => f.StationTaskRecord.Base_StationTaskId == _StationTaskDTO.StationTaskId);
            if (HasRepairData != null)
            {
                var hasRepair = HasRepairData.TightenReworks ?? new List<Proc_StationTask_TightenRework>();
                foreach (var item in hasRepair)
                {
                    var AutoScrewData = AutoScrewDataList.FirstOrDefault(f => f.OrderNo == item.OrderNo);
                    if (AutoScrewData == null)
                        continue;
                    AutoScrewData.Status = "OK";
                    AutoScrewData.ResultIsOK = true;
                }
            }

            productScrewDockData = App.GetProductScrewDockData();
            this.GridX.Value = productScrewDockData.LayOut_Width;
            this.GridY.Value = productScrewDockData.LayOut_Height;
            this.ImageSource.Value = new BitmapImage(new Uri(productScrewDockData.ImageUrl));
            var count = Math.Min(this.productScrewDockData.ScrewLocationList.Count, AutoScrewDataList.Count);
            for (int i = 0; i < count; i++)
            {
                this.productScrewDockData.ScrewLocationList[i].Status = AutoScrewDataList[i].ResultIsOK ? "OK" : "NG";
            }
            this.ScrewListObserver.ClearOnScheduler();
            this.ScrewListObserver.AddRangeOnScheduler(this.productScrewDockData.ScrewLocationList);

        }

        /// <summary>
        /// 绑定历史
        /// </summary>
        private void BindHistory()
        {
            var histDto = App.HisTaskData2.StationTaskRecords.FirstOrDefault(f => f.StationTaskId == _StationTaskDTO.StationTaskId);
            var timeRecord = histDto?.TightenReworks;
            if (timeRecord != null)
            {
                _StationTaskDTO.HasFinish = histDto.StationTaskRecord.Status == StationTaskStatusEnum.已完成;
            }
        }

        /// <summary>
        /// 找下一个NG螺丝
        /// </summary>
        public void FindNextNG()
        {
            if (AutoScrewDataList == null || AutoScrewDataList.Count == 0)
            {
                return;
            }

            if (AutoScrewDataList.All(a => a.ResultIsOK))
            {
                CompleteTask();
                return;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                CurDoingScrew = AutoScrewDataList.OrderBy(x => x.OrderNo).FirstOrDefault(a => !a.ResultIsOK && a.OrderNo > 0);
                if (CurDoingScrew == null)
                {
                    return;
                }

                CurDoingScrew.Status = "Doing";
                var screw = this.ScrewListObserver.FirstOrDefault(f => f.OrderNo == CurDoingScrew.OrderNo);
                if (screw != null)
                {
                    screw.Status = "Doing";
                }

                this.CurrentScrewNo.Value = CurDoingScrew.OrderNo;

                // 寻找下一颗需要补拧的螺丝（在当前螺丝之后），标记为 Next (橙色实线)
                var nextScrewData = AutoScrewDataList.OrderBy(x => x.OrderNo).FirstOrDefault(a => !a.ResultIsOK && a.OrderNo > CurDoingScrew.OrderNo);
                if (nextScrewData != null)
                {
                    nextScrewData.Status = "Next";
                    var nextScrewUI = this.ScrewListObserver.FirstOrDefault(f => f.OrderNo == nextScrewData.OrderNo);
                    if (nextScrewUI != null)
                    {
                        nextScrewUI.Status = "Next";
                    }
                }
            });
        }

        /// <summary>
        /// 使能
        /// </summary>
        public async void EnableBoltGuns()
        {
            if (AutoScrewDataList.All(a => a.ResultIsOK))
            {
                return;
            }

            //获取当前工位配置的拧紧枪
            var desoutterList = await _apiHelper.LoadStationProResourceConfig(ProResourceTypeEnum.拧紧枪);

            foreach (var desoutter in desoutterList)
            {
                if (string.IsNullOrEmpty(desoutter.DeviceNo))
                {
                    continue;
                }
                if (_taskReworkScrewBase.DevicesNos.Contains(desoutter.DeviceNo))
                {
                    _stationPLCContext.StationTightenStartReqs.Add(new StationPLCContext.StationTightenStartReq
                    {
                        DeviceNo = Convert.ToUInt16(desoutter.DeviceNo),
                        DeviceBrand = Convert.ToUInt16(desoutter.DeviceBrand == DeviceBrand.马头 ? 1 : 2),
                        ProgramNo = Convert.ToUInt16(_taskReworkScrewBase.ProgramNo),
                        SortNo = Convert.ToUInt16(CurrentScrewNo.Value),
                        ScrewCountBeforeCurrentTask = 0,
                        Resource = "补拧EnableBoltGuns"
                    });

                }
            }

        }

        public void EnableBoltGun(string deviceNo, int deviceBrand, int programNo, int sortNo)
        {
            if (_taskReworkScrewBase.DevicesNos.Contains(deviceNo))
            {
                _stationPLCContext.StationTightenStartReqs.Add(new StationPLCContext.StationTightenStartReq
                {
                    DeviceNo = Convert.ToUInt16(deviceNo),
                    DeviceBrand = Convert.ToUInt16(deviceBrand),
                    ProgramNo = Convert.ToUInt16(programNo),
                    SortNo = Convert.ToUInt16(sortNo),
                    ScrewCountBeforeCurrentTask = 0,
                    Resource = "补拧EnableBoltGun"

                });

            }
        }

        /// <summary>
        /// 处理拧紧枪消息
        /// </summary>
        /// <param name="request"></param>
        public async void CatchBoltGunMessage(BoltGunRequest request)
        {
            try
            {
                if (CurDoingScrew == null)
                {
                    await _mediator.Publish(new AlarmSYSNotification
                    {
                        Code = AlarmCode.拧紧枪错误, Name = nameof(AlarmCode.拧紧枪错误), Module = AlarmModule.SERVER_MODULE,
                        Description = $"没有需要补拧的螺栓"
                    });
                    return;
                }

                this.Torque.Value = $"{request.FinalTorque}N";
                this.Angle.Value = $"{request.FinalAngle}°";

                if (request.ProgramNo != _taskReworkScrewBase.ProgramNo)
                {
                    await _mediator.Publish(new AlarmSYSNotification
                    {
                        Code = AlarmCode.拧紧枪错误, Name = nameof(AlarmCode.拧紧枪错误), Module = AlarmModule.SERVER_MODULE,
                        Description = $"拧紧数据采集参数错误，请求的程序号{request.ProgramNo}，需要的程序号{_taskReworkScrewBase.ProgramNo}"
                    });
                    return;
                }

                var dto = new TightenReworkDataDto
                {
                    MainId = App.HisTaskData2.MainId,
                    TaskName = _taskReworkScrewBase.TaskName,
                    StationTaskId = _StationTaskDTO.StationTaskId,
                    Operator = App._RealtimePage.WorkingUserId,
                    ProgramNo = request.ProgramNo,
                    TorqueValue = request.FinalTorque,
                    AngleValue = request.FinalAngle,
                    TorqueMin = request.TorqueRate_Min,
                    TorqueMax = request.TorqueRate_Max,
                    AngleMin = request.Angle_Min,
                    AngleMax = request.Angle_Max,
                    ResultOk = request.ResultIsOK,
                    OrderNo = CurDoingScrew.OrderNo,
                    UpMesCode = _taskReworkScrewBase.UpMesCode + CurDoingScrew.OrderNo,
                    UpMesCodeJD = _taskReworkScrewBase.UpMesCode + "JD" + CurDoingScrew.OrderNo,
                };

                var validationResult = await ValidateTightingData(dto, request);
                if (!validationResult)
                {
                    return;
                }

                var saveResult = await _apiHelper.SaveRepairData(dto);
                if (saveResult.Code != 200)
                {
                    var notice = new AlarmSYSNotification
                    {
                        Code = AlarmCode.系统运行错误,
                        Name = nameof(AlarmCode.系统运行错误),
                        Module = AlarmModule.SERVER_MODULE,
                        Description = $"保存失败，错误信息：{saveResult.Message}；MainId={dto.MainId}；StationTaskId={dto.StationTaskId}；OrderNo={dto.OrderNo}"
                    };
                    await _mediator.Publish(notice);
                    return;
                }
                CurDoingScrew.Status = null;
                CurDoingScrew.ResultIsOK = true;
                var screw = this.ScrewListObserver.FirstOrDefault(f => f.OrderNo == CurDoingScrew.OrderNo);
                if (screw != null)
                {
                    screw.Status = null;
                }
                FindNextNG();

                //没拧完，继续上使能
                if (AutoScrewDataList.Any(a => !a.ResultIsOK))
                {
                    EnableBoltGun(request.DeviceNo, request.DeviceBrand, request.ProgramNo, CurrentScrewNo.Value);
                }

                //补拧弹框输入螺丝号
                //Application.Current.Dispatcher.Invoke(() =>
                //{
                //    using var scope = _serviceProvider.CreateScope();
                //    var sp = scope.ServiceProvider;
                //    var confirmPage = sp.GetRequiredService<ReTightenNoConfirmPage>();
                //    ((ReTightenNoConfirmViewModel)confirmPage.DataContext).TightenNo = CurDoingScrew.OrderNo.ToString();
                //    ((ReTightenNoConfirmViewModel)confirmPage.DataContext).Action = async delegate
                //    {
                //        confirmPage.Close();
                //        confirmPage.Visibility = Visibility.Hidden;
                //        var saveResult = await _apiHelper.SaveRepairData(dto);
                //        if (saveResult.Code != 200)
                //        {
                //            var notice = new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"保存失败，请联系管理员！" };
                //            await this._mediator.Publish(notice);
                //            return;
                //        }
                //        CurDoingScrew.Status = null;
                //        CurDoingScrew.ResultIsOK = true;
                //        var screw = this.ScrewListObserver.FirstOrDefault(f => f.OrderNo == CurDoingScrew.OrderNo);
                //        if (screw != null)
                //        {
                //            screw.Status = null;
                //        }
                //        FindNextNG();

                //        //没拧完，继续上使能
                //        if (AutoScrewDataList.Any(a => !a.ResultIsOK))
                //        {
                //            EnableBoltGun(request.DeviceNo, request.DeviceBrand, request.ProgramNo, CurrentScrewNo.Value);
                //        }
                //    };
                //    confirmPage.ShowDialog();
                //});

            }
            catch (Exception ex)
            {
                await _mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.DESOUTTER_MODULE, Description = $"{ex.Message}/{ex.StackTrace}" });
            }
        }

        private async Task<bool> ValidateTightingData(TightenReworkDataDto dto, BoltGunRequest request)
        {
            var task = _taskReworkScrewBase;

            //判断拧紧数据是否异常
            if (!request.ResultIsOK)
            {
                await _mediator.Publish(new AlarmSYSNotification
                {
                    Code = AlarmCode.拧紧枪错误,
                    DeviceNo = request.DeviceNo,
                    Name = nameof(AlarmCode.拧紧NG),
                    Module = "拧紧NG",
                    PackCode = task.PackCode,
                    Description = $"拧紧NG，请复位后重拧！\r\n任务:{task.StationTask.Name}\r\n枪号:{request.DeviceNo}, 程序号:{task.ProgramNo}",
                    TightenNGExtra = new AlarmExtra.TightenNG { DeviceNo = request.DeviceNo, ScrewSerialNo = dto.OrderNo }
                });
                return false;
            }
            // 校验角度和扭矩是否相等
            if (dto.AngleValue == dto.TorqueValue)
            {
                await _mediator.Publish(new AlarmSYSNotification
                {
                    Code = AlarmCode.拧紧枪错误,
                    DeviceNo = request.DeviceNo,
                    Name = nameof(AlarmCode.拧紧NG),
                    Module = "拧紧NG",
                    PackCode = task.PackCode,
                    Description = $"拧紧角度[{dto.AngleValue}]和扭矩[{dto.TorqueValue}]相同！请复位后重拧！\r\n任务:{task.StationTask.Name}\r\n枪号:{request.DeviceNo}, 程序号:{task.ProgramNo}",
                    TightenNGExtra = new AlarmExtra.TightenNG { DeviceNo = request.DeviceNo, ScrewSerialNo = dto.OrderNo }
                });
                return false;
            }
            if (dto.AngleValue == 0 || dto.TorqueValue == 0)
            {
                await _mediator.Publish(new AlarmSYSNotification
                {
                    Code = AlarmCode.拧紧枪错误,
                    DeviceNo = request.DeviceNo,
                    Name = nameof(AlarmCode.拧紧NG),
                    Module = "拧紧NG",
                    PackCode = task.PackCode,
                    Description = $"拧紧角度[{dto.AngleValue}]或扭矩[{dto.TorqueValue}]为 0！请复位后重拧！\r\n任务:{task.StationTask.Name}\r\n枪号:{request.DeviceNo}, 程序号:{task.ProgramNo}",
                    TightenNGExtra = new AlarmExtra.TightenNG { DeviceNo = request.DeviceNo, ScrewSerialNo = dto.OrderNo }
                });
                return false;
            }
            //校验角度是否超限
            if (request.FinalAngle < task.MinAngle || request.FinalAngle > task.MaxAngle)
            {
                await _mediator.Publish(new AlarmSYSNotification
                {
                    Code = AlarmCode.拧紧枪错误,
                    DeviceNo = request.DeviceNo,
                    Name = nameof(AlarmCode.拧紧NG),
                    Module = "拧紧NG",
                    PackCode = task.PackCode,
                    Description = $"拧紧角度超限[{task.MinAngle}, {task.MaxAngle}]，请复位后重拧！\r\n任务:{task.StationTask.Name}\r\n枪号:{request.DeviceNo}, 程序号:{task.ProgramNo}",
                    TightenNGExtra = new AlarmExtra.TightenNG { DeviceNo = request.DeviceNo, ScrewSerialNo = dto.OrderNo }
                });
                return false;
            }
            //校验扭矩是否超限
            if (request.FinalTorque < task.MinTorque || request.FinalTorque > task.MaxTorque)
            {
                await _mediator.Publish(new AlarmSYSNotification
                {
                    Code = AlarmCode.拧紧枪错误,
                    DeviceNo = request.DeviceNo,
                    Name = nameof(AlarmCode.拧紧NG),
                    Module = "拧紧NG",
                    PackCode = task.PackCode,
                    Description = $"拧紧扭矩超限[{task.MinTorque}, {task.MaxTorque}]，请复位后重拧！\r\n任务:{task.StationTask.Name}\r\n枪号:{request.DeviceNo}, 程序号:{task.ProgramNo}",
                    TightenNGExtra = new AlarmExtra.TightenNG { DeviceNo = request.DeviceNo, ScrewSerialNo = dto.OrderNo }
                });
                return false;
            }

            return true;
        }

        /// <summary>
        /// 当前任务完成
        /// </summary>
        private void CompleteTask()
        {
            _StationTaskDTO.PackCode = App.PackBarCode;
            _StationTaskDTO.AGVCode = App.AGVCode;
            _StationTaskDTO.UpdateUserID = App._RealtimePage.WorkingUserId;
            _StationTaskDTO.UpdateTime = DateTime.Now;
            _StationTaskDTO.HasFinish = true;
            //DisableBoltGuns();
            OnCompleteTask(_StationTaskDTO);
        }

        /// <summary>
        /// 初始化螺丝数据集合
        /// </summary>
        /// <param name="limtcount"></param>
        public void InitData(int limtcount)
        {
            // 初始化螺丝数据集合
            for (int i = 0; i < limtcount; i++)
            {
                _AutoScrewDataList.Add(new Proc_AutoBoltInfo_Detail_DTO_Ext());
            }
        }

        public StationTaskDTO _StationTaskDTO { get; }

        public ObservableCollection<Proc_AutoBoltInfo_Detail_DTO_Ext> AutoScrewDataList
        {
            get => _AutoScrewDataList;
            set
            {
                if (_AutoScrewDataList != value)
                {
                    _AutoScrewDataList = value;
                    OnPropertyChanged(nameof(AutoScrewDataList));
                }
            }
        }
        private ObservableCollection<Proc_AutoBoltInfo_Detail_DTO_Ext> _AutoScrewDataList = new();

        private ProductScrewDockData productScrewDockData { get; set; }

        public ReactiveProperty<double> GridX { get; }

        public ReactiveProperty<double> GridY { get; }

        public ReactiveProperty<BitmapImage> ImageSource { get; }

        public ReactiveCollection<ScrewLocation> ScrewListObserver { get; }

        public ReactiveProperty<string> Torque { get; }

        public ReactiveProperty<string> Angle { get; }

        public ReactiveProperty<int> CurrentScrewNo { get; }

        public ReactiveProperty<double> FlipX { get; }
        public ReactiveProperty<double> FlipY { get; }
        public ReactiveProperty<double> PageScale { get; }
    }
}
