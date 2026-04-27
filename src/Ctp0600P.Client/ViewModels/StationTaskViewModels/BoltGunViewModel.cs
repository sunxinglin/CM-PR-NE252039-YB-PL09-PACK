using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using Ctp0600P.Client.PLC.Context;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.Protocols.BoltGun.Models;
using Ctp0600P.Shared;

using MediatR;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using Reactive.Bindings;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;

namespace Ctp0600P.Client.ViewModels.StationTaskViewModels;

public class BoltGunViewModel : TaskViewModelBase
{
    private readonly IMediator _mediator;
    private readonly IServiceProvider _service;
    private readonly IAPIHelper _apiHelper;
    private readonly StationPLCContext _manualStationNotifyPLCContext;
    private readonly int _screwCountBeforeCurrentTask;

    private StationTaskDTO StationTaskDTO { get; }

    public ReactiveProperty<double> PageScale { get; }

    public ObservableCollection<Base_StationTaskScrew> StationTaskScrewList
    {
        get => _StationTaskScrewList;
        set
        {
            if (_StationTaskScrewList != value)
            {
                _StationTaskScrewList = value;
                OnPropertyChanged(nameof(StationTaskScrewList));
            }
        }
    }
    private ObservableCollection<Base_StationTaskScrew> _StationTaskScrewList = new();

    public BoltGunViewModel(StationTaskDTO stationTaskDTO, ObservableCollection<Base_StationTaskScrew> taskScrew, IMediator mediator, IServiceProvider service, IAPIHelper apiHelper, StationPLCContext manualStationNotifyPLCContext, int screwCountBeforeCurrentTask, IOptionsMonitor<PageScaleConfig> pageScaleConfig)
    {
        _mediator = mediator;
        _service = service;
        _apiHelper = apiHelper;
        StationTaskDTO = stationTaskDTO;
        StationTaskScrewList = taskScrew;

        PageScale = new ReactiveProperty<double> { Value = 1.0 };
        pageScaleConfig.OnChange(settings =>
        {
            PageScale.Value = settings.BoltGun > 0 ? settings.BoltGun : 1.0;
        });
        PageScale.Value = pageScaleConfig.CurrentValue.BoltGun > 0 ? pageScaleConfig.CurrentValue.BoltGun : 1.0;

        BindHisData();
        _manualStationNotifyPLCContext = manualStationNotifyPLCContext;
        _screwCountBeforeCurrentTask = screwCountBeforeCurrentTask;
    }

    /// <summary>
    /// 绑定历史数据
    /// </summary>
    public void BindHisData()
    {
        var histDto = App.HisTaskData2.StationTaskRecords.FirstOrDefault(f => f.StationTaskId == StationTaskDTO.StationTaskId);
        var screws = histDto?.Screws;
        if (screws is { Count: > 0 })
        {
            StationTaskDTO.HasFinish = histDto.StationTaskRecord.Status == StationTaskStatusEnum.已完成;
            foreach (var screw in screws)
            {
                var s = StationTaskScrewList.FirstOrDefault(f => f.Id == screw.StationTaskScrew.Base_ScrewTaskId);
                if (s == null) continue;

                s.HasPassed = screw.StationTaskScrew.Status == StationTaskStatusEnum.已完成;
                s.CurNo = screw.ScrewDetails.Where(w => !w.IsDeleted && w.ResultIsOK).Count();
                s.CurentScrewNo = screw.StationTaskScrew.Status == StationTaskStatusEnum.已完成 ? s.UpMESCodeStartNo + 1 : GetScrewNo(s, StationTaskDTO.StationTaskId);
            }
        }
    }

    /// <summary>
    /// 绑定历史信息
    /// </summary>
    /// <param name="record"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void BindHisData(Proc_StationTask_Record record)
    {
        try
        {
            foreach (var screw in StationTaskScrewList)
            {
                var hisRecord = record.Proc_StationTask_BlotGuns.FirstOrDefault(h => h.ProgramNo == screw.ProgramNo && h.ScrewName == screw.ScrewSpecs);
                if (hisRecord == null) continue;
                screw.Status = hisRecord.Status.ToString();
                screw.CurNo = hisRecord.CurCompleteNum;
                screw.HasPassed = hisRecord.Status == StationTaskStatusEnum.已完成;
                foreach (var screwDetail in hisRecord.Proc_StationTask_BlotGunDetails)
                {
                    screw.ScrewResultList.Add(new ScrewResult
                    {
                        DeviceNo = screwDetail.DeviceNo.ToString(), ScrewState = screwDetail.ResultIsOK ? "OK" : "NG",
                        ScrewValue = screwDetail.FinalTorque.ToString(CultureInfo.InvariantCulture)
                    });
                }
            }
        }
        catch (Exception ex)
        {
            _mediator.Publish(new AlarmSYSNotification
            {
                Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.SERVER_MODULE,
                Description = $"绑定拧紧历史数据失败，请联系管理员！"
            });
        }

    }

    /// <summary>
    /// 上使能。如果是复位上使能，则按照传过来的deviceNos给使能
    /// </summary>
    public async void EnableBoltGuns(bool resetEnable = false, params string[] deviceNos)
    {
        //获取当前工位配置的拧紧枪
        var desoutterList = await _apiHelper.LoadStationProResourceConfig(ProResourceTypeEnum.拧紧枪);

        foreach (var desoutter in desoutterList)
        {
            if (string.IsNullOrEmpty(desoutter.DeviceNo))
            {
                continue;
            }
            var task = StationTaskScrewList.FirstOrDefault(s => s.DeviceNos.Contains(desoutter.DeviceNo));
            if (task == null)
            {
                //找不到任务，不上使能
                continue;
            }
            if (task.Status == "已完成")
            {
                //任务已完成，不上使能
                continue;
            }
            if (resetEnable && !deviceNos.Contains(desoutter.DeviceNo))
            {
                //复位过来的，但枪号不在复位的报警里，不上使能
                continue;
            }
            //上使能
            _manualStationNotifyPLCContext.StationTightenStartReqs.Add(new StationPLCContext.StationTightenStartReq
            {
                DeviceNo = Convert.ToUInt16(desoutter.DeviceNo),
                DeviceBrand = Convert.ToUInt16(desoutter.DeviceBrand == DeviceBrand.马头 ? 1 : 2),
                ProgramNo = Convert.ToUInt16(task.ProgramNo),
                SortNo = Convert.ToUInt16(_screwCountBeforeCurrentTask + task.UpMESCodeStartNo + task.CurNo + 1),
                ScrewCountBeforeCurrentTask = _screwCountBeforeCurrentTask,
                Resource = $"[{DateTimeOffset.Now.UtcTicks}]拧紧EnableBoltGuns"
            });
        }
    }

    public void EnableBoltGun(int deviceNo, int deviceBrand, int sortNo, int screwCountBeforeCurrentTask)
    {
        var task = StationTaskScrewList.FirstOrDefault(s => s.DeviceNos.Contains(deviceNo.ToString()));
        if (task != null && task.Status != "已完成")
        {
            _manualStationNotifyPLCContext.StationTightenStartReqs.Add(new StationPLCContext.StationTightenStartReq
            {
                DeviceNo = Convert.ToUInt16(deviceNo),
                DeviceBrand = Convert.ToUInt16(deviceBrand),
                ProgramNo = Convert.ToUInt16(task.ProgramNo),
                SortNo = Convert.ToUInt16(sortNo),
                ScrewCountBeforeCurrentTask = screwCountBeforeCurrentTask,
                Resource = $"[{DateTimeOffset.Now.UtcTicks}]拧紧EnableBoltGun"
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
            // 身份核对：根据程序号(ProgramNo)和枪号(DeviceNo)找到当前正在拧的那颗螺丝
            var Screw = CheckScrew(request);
            if (Screw == null)
            {
                await _mediator.Publish(new AlarmSYSNotification
                {
                    Code = AlarmCode.拧紧枪错误, Name = nameof(AlarmCode.拧紧枪错误), Module = AlarmModule.DESOUTTER_MODULE,
                    Description =
                        $"收到拧紧完成信息，但找不到设备号DeviceNo={request.DeviceNo} 程序号ProgramNo={request.ProgramNo} 的任务。{Environment.NewLine}任务数据：{JsonConvert.SerializeObject(StationTaskScrewList)}，收到的请求：{JsonConvert.SerializeObject(request)}"
                });
                return;
            }

            if (Screw.CurNo == Screw.UseNum)
            {
                return;
            }
            Screw.CurScrewValue = request.FinalTorque;
            Screw.CurScrewValueStr = "扭力:" + request.FinalTorque + "N";
            Screw.CurScrewState = request.ResultIsOK ? "OK" : "NG";
            Screw.CurAngle_Max = request.Angle_Max;
            Screw.CurAngle_Min = request.Angle_Min;
            Screw.CurAngleStatus = request.AngleStatus;
            Screw.CurFinalAngle = request.FinalAngle;
            Screw.CurFinalAngleStr = "角度:" + request.FinalAngle;
            Screw.CurAngle_MaxStr = "上限:" + request.Angle_Max;
            Screw.CurAngle_MinStr = "下限:" + request.Angle_Min;
            Screw.CurTargetAngle = request.TargetAngle;
            Screw.CurFinalTorque = request.FinalTorque;
            Screw.CurTargetTorqueRate = request.TargetTorqueRate;
            Screw.CurTorqueRate_Max = request.TorqueRate_Max;
            Screw.CurTorqueRate_Min = request.TorqueRate_Min;
            if (int.TryParse(request.DeviceNo, out var deviceNo))
            {
                Screw.DeviceNo = deviceNo;
            }
            // 质量判定：校验拧紧数据（角度、扭力）
            Screw.ResultIsOK = await ValidateTighteningData(Screw, request);
            
            Screw.CurNo++;
            // 保存到数据库
            bool saveOK = await SaveScrewData(Screw, StationTaskDTO.StationTaskId);
            if (!saveOK || !Screw.ResultIsOK)
            {
                Screw.CurNo--;
            }
            if (!saveOK)
            {
                await _mediator.Publish(new AlarmSYSNotification
                {
                    Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.DESOUTTER_MODULE,
                    Description = $"拧紧数据保存失败，请联系管理员！"
                });
                return;
            }
            // 拧紧失败，防止继续上使能
            if (!Screw.ResultIsOK)
            {
                return;
            }

            Screw.ScrewResultList.Add(new ScrewResult { DeviceNo = request.DeviceNo, ScrewState = Screw.CurScrewState, ScrewValue = Screw.CurScrewValueStr });
            
            if (Screw.CurNo >= Screw.UseNum)
            {
                // 触发当站任务全部完成逻辑
                CompleteTask(Screw);
                return;
            }

            // 计算螺丝序号
            var screwNo = _screwCountBeforeCurrentTask + Screw.UpMESCodeStartNo + Screw.CurNo + 1;

            // 上使能
            EnableBoltGun(Convert.ToUInt16(request.DeviceNo), request.DeviceBrand, screwNo, _screwCountBeforeCurrentTask);
        }
        catch (Exception ex)
        {
            await _mediator.Publish(new AlarmSYSNotification
            {
                Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.DESOUTTER_MODULE,
                Description = $"{ex.Message}/{ex.StackTrace}"
            });
        }
    }

    private async Task<bool> ValidateTighteningData(Base_StationTaskScrew task, BoltGunRequest request)
    {
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
                TightenNGExtra = new AlarmExtra.TightenNG { DeviceNo = request.DeviceNo, ScrewSerialNo = _screwCountBeforeCurrentTask + task.UpMESCodeStartNo + task.CurNo + 1 }
            });
            return false;
        }
        // 校验角度和扭矩是否相等
        if (task.CurFinalAngle == task.CurFinalTorque)
        {
            await _mediator.Publish(new AlarmSYSNotification
            {
                Code = AlarmCode.拧紧枪错误,
                DeviceNo = request.DeviceNo,
                Name = nameof(AlarmCode.拧紧NG),
                Module = "拧紧NG",
                PackCode = task.PackCode,
                Description = $"拧紧角度[{task.CurFinalAngle}]和扭矩[{task.CurFinalTorque}]相同！请复位后重拧！\r\n任务:{task.StationTask.Name}\r\n枪号:{request.DeviceNo}, 程序号:{task.ProgramNo}",
                TightenNGExtra = new AlarmExtra.TightenNG { DeviceNo = request.DeviceNo, ScrewSerialNo = _screwCountBeforeCurrentTask + task.UpMESCodeStartNo + task.CurNo + 1 }
            });
            return false;
        }

        if (task.CurFinalAngle == 0 || task.CurFinalTorque == 0)
        {
            await _mediator.Publish(new AlarmSYSNotification
            {
                Code = AlarmCode.拧紧枪错误,
                DeviceNo = request.DeviceNo,
                Name = nameof(AlarmCode.拧紧NG),
                Module = "拧紧NG",
                PackCode = task.PackCode,
                Description = $"拧紧角度[{task.CurFinalAngle}]或扭矩[{task.CurFinalTorque}]为 0！请复位后重拧！\r\n任务:{task.StationTask.Name}\r\n枪号:{request.DeviceNo}, 程序号:{task.ProgramNo}",
                TightenNGExtra = new AlarmExtra.TightenNG { DeviceNo = request.DeviceNo, ScrewSerialNo = _screwCountBeforeCurrentTask + task.UpMESCodeStartNo + task.CurNo + 1 }
            });
            return false;
        }

        //校验角度是否超限
        if ((task.AngleMinLimit.HasValue && request.FinalAngle < task.AngleMinLimit)
            || (task.AngleMaxLimit.HasValue && request.FinalAngle > task.AngleMaxLimit))
        {
            await _mediator.Publish(new AlarmSYSNotification
            {
                Code = AlarmCode.拧紧枪错误,
                DeviceNo = request.DeviceNo,
                Name = nameof(AlarmCode.拧紧NG),
                Module = "拧紧NG",
                PackCode = task.PackCode,
                Description = $"拧紧角度超限[{task.AngleMinLimit}, {task.AngleMaxLimit}]，请复位后重拧！\r\n任务:{task.StationTask.Name}\r\n枪号:{request.DeviceNo}, 程序号:{task.ProgramNo}",
                TightenNGExtra = new AlarmExtra.TightenNG { DeviceNo = request.DeviceNo, ScrewSerialNo = _screwCountBeforeCurrentTask + task.UpMESCodeStartNo + task.CurNo + 1 }
            });
            return false;
        }
        //校验扭矩是否超限
        if ((task.TorqueMinLimit.HasValue && request.FinalTorque < task.TorqueMinLimit)
            || (task.TorqueMaxLimit.HasValue && request.FinalTorque > task.TorqueMaxLimit))
        {
            await _mediator.Publish(new AlarmSYSNotification
            {
                Code = AlarmCode.拧紧枪错误,
                DeviceNo = request.DeviceNo,
                Name = nameof(AlarmCode.拧紧NG),
                Module = "拧紧NG",
                PackCode = task.PackCode,
                Description = $"拧紧扭矩超限[{task.TorqueMinLimit}, {task.TorqueMaxLimit}]，请复位后重拧！\r\n任务:{task.StationTask.Name}\r\n枪号:{request.DeviceNo}, 程序号:{task.ProgramNo}",
                TightenNGExtra = new AlarmExtra.TightenNG { DeviceNo = request.DeviceNo, ScrewSerialNo = _screwCountBeforeCurrentTask + task.UpMESCodeStartNo + task.CurNo + 1 }
            });
            return false;
        }

        return true;
    }

    /// <summary>
    /// 当前任务完成
    /// </summary>
    /// <param name="hasGoods"></param>
    private async void CompleteTask(Base_StationTaskScrew Screw)
    {
        Screw.Status = "已完成";
        Screw.HasPassed = true;

        foreach (var eachScrew in StationTaskScrewList)
        {
            if (!eachScrew.HasPassed) return;
        }

        bool saveOK = await SetScrewTaskFinish(Screw);
        if (saveOK)
        {
            if (StationTaskScrewList.Where(s => s.HasPassed).ToList().Count == StationTaskScrewList.Count)
            {
                StationTaskDTO.HasFinish = true;
                OnCompleteTask(StationTaskDTO);
            }
        }
        else
        {
            var notice = new MessageNotice
                { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"保存失败，请联系管理员！" };
            await _mediator.Publish(notice);
            await _mediator.Publish(new AlarmSYSNotification
            {
                Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.DESOUTTER_MODULE,
                Description = $"拧紧数据保存失败，请联系管理员！"
            });
        }
    }

    /// <summary>
    /// 保存拧紧数据
    /// </summary>
    /// <param name="screw"></param>
    /// <param name="taskId"></param>
    /// <returns></returns>
    public async Task<bool> SaveScrewData(Base_StationTaskScrew screw, int taskId)
    {
        screw.CreateUserID = App._RealtimePage.WorkingUserId;
        return await App._RealtimePage.SaveScrewData(screw, taskId);
    }

    /// <summary>
    /// 设置拧紧任务完成
    /// </summary>
    /// <param name="screw"></param>
    /// <returns></returns>
    public async Task<bool> SetScrewTaskFinish(Base_StationTaskScrew screw)
    {
        screw.CreateUserID = App._RealtimePage.WorkingUserId;
        return await App._RealtimePage.SetScrewTaskFinish(screw);
    }

    /// <summary>
    /// 校验并获取拧紧任务
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public Base_StationTaskScrew CheckScrew(BoltGunRequest request)
    {
        foreach (var screw in StationTaskScrewList)
        {
            if (screw.DeviceNos.Split(',').Contains(request.DeviceNo))
            {
                if (screw.ProgramNo == request.ProgramNo)
                {
                    //var lResource = screw.StationTask.ListResource.FirstOrDefault(r => r.DeviceNo == request.DeviceNo);
                    //if (lResource == null) 
                    //{
                    //    return null;
                    //}
                    //screw.UseResourceId = lResource.Id;
                    return screw;
                }
            }
        }
        return null;
    }

    public int GetScrewNo(Base_StationTaskScrew screw, int taskId)
    {
        var detail = screw.ScrewResultList.OrderBy(o => o.OrderNo).ToList();
        var noOrder = detail.FirstOrDefault(f => f.OrderNo != detail.IndexOf(f) + 1 + screw.UpMESCodeStartNo);
        if (noOrder == null)
        {
            return detail.Count + 1 + screw.UpMESCodeStartNo;
        }
        return detail.IndexOf(noOrder) + 1 + screw.UpMESCodeStartNo;
    }
    
    public class LastTighteningID
    {
        public int devicesNo { get; set; }
        public long TighteningID { get; set; }
    }
    
}
