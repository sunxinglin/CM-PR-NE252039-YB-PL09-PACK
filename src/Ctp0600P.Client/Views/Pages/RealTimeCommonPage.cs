using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using AsZero.Core.Services.Messages;

using Ctp0600P.Client.Apis;
using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.ViewModels;
using Ctp0600P.Client.Views.StationTaskPages;

using MediatR;

using Microsoft.Extensions.Logging;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DTOS;
using Yee.Entitys.DTOS.StationTaskDataDTOS;
using Yee.Entitys.Production;
using Yee.Services.CatlMesInvoker;

namespace Ctp0600P.Client.Views.Pages;

/// <summary>
/// StationTaskCommonPage.xaml 的交互逻辑
/// </summary>
public class RealTimeCommonPage : Page
{
    protected APIHelper ApiHelper;
    protected ICatlMesInvoker CatlMesInvoker;
    public RealtimePageViewModel Vm;
    public int WorkingUserId;
    public string WorkingUserCard { get; set; } = "";
    private readonly IMediator _mediator;

    private SemaphoreSlim _slim = new(1, 1);

    protected RealTimeCommonPage(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task BuildUpMesCollectData()
    {
        if (App.ActivePage != null && App.ActivePage.GetType() == typeof(LetGoPage))
        {
            var data = await ApiHelper.GetUploadCATLData();
            if (data is { Code: 200, Result: not null })
            {
                App.UpCatlMesData = data.Result;
            }
            else if (data is { Code: 500, Result: null })
            {
                var notice = new MessageNotice
                {
                    messageType = MessageTypeEnum.提示, showType = MessageShowType.右下角弹窗,
                    MessageStr = "组织构建上传数据包超时，正在重试中..."
                };
                await _mediator.Publish(notice);

                await BuildUpMesCollectData();
            }
        }
    }

    public async Task<bool> CallCatlMesCollectDataFucAsync()
    {
        if (App._StepStationSetting.IsDebug)
        {
            return true;
        }

        // 加载上传数据
        var res = await ApiHelper.GetUploadCATLData();
        var uploadData = res.Result;

        if (uploadData == null || !uploadData.DCParams.Any())
        {
            var msg = new UILogNotification
            {
                LogMessage = new LogMessage
                    { Content = "没有需要上传的收数数据，不调用收数接口！", Level = LogLevel.Information, Timestamp = DateTime.Now }
            };
            await _mediator.Publish(msg);
            return true;
        }

        var stationCode = !string.IsNullOrEmpty(App.CurrentStationCode)
            ? App.CurrentStationCode
            : App._StepStationSetting.StationCode;
        var cRes = await CatlMesInvoker.dataCollect(uploadData.PackCode, uploadData.DCParams, true, stationCode);
        if (cRes.code != 0)
        {
            var cAlarm = new AlarmSYSNotification
            {
                Code = AlarmCode.CatlMES错误, Name = nameof(AlarmCode.CatlMES错误),
                Description = $"(Code={cRes.code}) {cRes.message}"
            };
            await _mediator.Publish(cAlarm);
            return false;
        }

        var cSuccessMsg = new UILogNotification
        {
            LogMessage = new LogMessage
                { Content = "数据上传CATL成功！", Level = LogLevel.Information, Timestamp = DateTime.Now }
        };
        await _mediator.Publish(cSuccessMsg);
        return true;
    }
    
    public async Task<CatlMESReponse> TryNc(string ncCode)
    {
        return await CatlMesInvoker.AutoNc(App.PackBarCode, App.CurrentStationCode, ncCode);
    }

    /// <summary>
    /// 当前工位任务完成通知
    /// </summary>
    /// <param name="dto"></param>
    public async Task RealtimePage_CompleteTask(StationTaskDTO dto)
    {
        // 防抖
        if (await _slim.WaitAsync(30) == false)
        {
            return;
        }

        try
        {
            bool saveStatusOK = false;
            if (dto.StationTask.Type == StationTaskTypeEnum.放行)
            {
                saveStatusOK = await SetStationCurTaskRunAGV(dto);
                dto.HasFinish = saveStatusOK;
            }
            else
            {
                // saveStatusOK = await SetStationCurTaskFinish(dto, App.HisTaskData2.MainId);
                var alreadyFinished = App.HisTaskData2?.StationTaskRecords?.Any(r =>
                    r.StationTaskRecord.Base_StationTaskId == dto.StationTaskId &&
                    r.StationTaskRecord.Status == StationTaskStatusEnum.已完成) == true;
                if (alreadyFinished)
                {
                    saveStatusOK = true;
                }
                else
                {
                    saveStatusOK = await SetStationCurTaskFinish(dto, App.HisTaskData2.MainId);
                }
            }

            if (saveStatusOK)
            {
                bool hasFinish = Vm.CheckStationTaskFinish(dto);
                if (hasFinish)
                {
                    if (App._StepStationSetting.StepType == StepTypeEnum.线外人工站)
                    {
                        Vm.InitEmpty();
                    }
                    else
                    {
                        Vm.DealRunAGV();
                        Vm.InitEmpty();
                    }
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Vm.GoNextUndoTask();
                        Vm.DealTaskStep();
                    });
                }
            }
            // 这段代码可能会导致报警2次
            else
            {
                await _mediator.Publish(new AlarmSYSNotification
                {
                    Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.SERVER_MODULE,
                    Description = $"[{dto.StationTask.Type}]保存失败，请联系管理员！方法名：RealtimePage_CompleteTask()"
                });
            }
        }
        finally
        {
            _slim.Release();
        }
    }

    /// <summary>
    /// 保存扫描员工卡信息
    /// </summary>
    /// <param name="taskScanAccountCard"></param>
    /// <returns></returns>
    public async Task<bool> SaveAccountCardData(Base_StationTaskScanAccountCard taskScanAccountCard, int taskId)
    {
        var response = await ApiHelper.SaveAccountCardData(taskScanAccountCard, taskId);
        if (response.Code == 200)
        {
            Vm.AddLogMsg.Execute(new LogMessage { Timestamp = DateTime.Now, Content = "保存扫描员工卡数据成功" });
            return true;
        }

        await _mediator.Publish(new AlarmSYSNotification
        {
            Code = AlarmCode.扫码枪错误, Name = nameof(AlarmCode.扫码枪错误), Module = AlarmModule.SERVER_MODULE,
            Description = $"保存扫描员工卡失败{response.Message}"
        });
        return false;
    }

    /// <summary>
    /// 放行
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    private async Task<bool> SetStationCurTaskRunAGV(StationTaskDTO dto)
    {
        var response = await ApiHelper.SetStationCurTaskRunAGV(dto);
        if (response.Code == 200)
        {
            if (!string.IsNullOrEmpty(App.AGVCode))
            {
                Vm.AddLogMsg.Execute(new LogMessage
                    { Timestamp = DateTime.Now, Content = $"【{dto.StationTask.Name}】完成，AGV：{App.AGVCode}" });
            }

            return true;
        }

        await _mediator.Publish(new AlarmSYSNotification
        {
            Code = AlarmCode.AGV错误, Name = nameof(AlarmCode.AGV错误), Module = AlarmModule.AGV_MODULE,
            Description = $"【{dto.StationTask.Name}】失败，AGV：{App.AGVCode},{response.Message}"
        });

        return false;
    }

    private async Task<bool> SetStationCurTaskFinish(StationTaskDTO dto, int mainId)
    {
        var response = await ApiHelper.SetStationCurTaskFinish(dto, mainId);
        if (response.Code == 200)
        {
            Vm.SetStationCurTaskFinish(dto);
            Vm.AddLogMsg.Execute(new LogMessage { Timestamp = DateTime.Now, Content = $"【{dto.StationTask.Name}】完成" });
            return true;
        }

        await _mediator.Publish(new AlarmSYSNotification
        {
            Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.SERVER_MODULE,
            Description = $"【{dto.StationTask.Name}】失败：{response.Message}"
        });
        return false;
    }

    /// <summary>
    /// 保存电子秤数据
    /// </summary>
    /// <param name="taskAnyLoad"></param>
    /// <returns></returns>
    public async Task<bool> SaveAnyLoadData(Base_StationTaskAnyLoad taskAnyLoad, int taskId)
    {
        var response = await ApiHelper.SaveAnyLoadData(taskAnyLoad, taskId);
        if (response.Code == 200)
        {
            Vm.AddLogMsg.Execute(new LogMessage { Timestamp = DateTime.Now, Content = "保存称重数据成功" });
            return true;
        }

        await _mediator.Publish(new AlarmSYSNotification
        {
            Code = AlarmCode.称重错误, Name = nameof(AlarmCode.称重错误), Module = AlarmModule.SERVER_MODULE,
            Description = $"保存称重数据失败"
        });
        return false;
    }

    /// <summary>
    /// 保存 用户输入 数据
    /// </summary>
    /// <param name="taskUserInput"></param>
    /// <param name="taskId"></param>
    /// <returns></returns>
    public async Task<bool> SaveUserInputData(Base_StationTaskUserInput taskUserInput, int taskId)
    {
        var response = await ApiHelper.Save_StationUserScan(taskUserInput, taskId);
        if (response.Code == 200)
        {
            Vm.AddLogMsg.Execute(new LogMessage { Timestamp = DateTime.Now, Content = "保存用户输入数据数据成功" });
            return true;
        }

        await _mediator.Publish(new AlarmSYSNotification
        {
            Code = AlarmCode.用户输入错误, Name = nameof(AlarmCode.用户输入错误), Module = AlarmModule.SERVER_MODULE,
            Description = "保存用户输入数据失败"
        });
        return false;
    }

    /// <summary>
    /// 保存 扫码输入 数据
    /// </summary>
    /// <param name="taskUserScan"></param>
    /// <param name="taskId"></param>
    /// <returns></returns>
    public async Task<bool> SaveScanCollectData(Base_StationTaskScanCollect taskUserScan, int taskId)
    {
        var response = await ApiHelper.Save_StationScanCollect(taskUserScan, taskId);
        if (response.Code == 200)
        {
            Vm.AddLogMsg.Execute(new LogMessage { Timestamp = DateTime.Now, Content = "保存扫码输入数据数据成功" });
            return true;
        }

        await _mediator.Publish(new AlarmSYSNotification
        {
            Code = AlarmCode.用户输入错误, Name = nameof(AlarmCode.用户输入错误), Module = AlarmModule.SERVER_MODULE,
            Description = $"保存用户输入数据失败"
        });
        return false;
    }

    /// <summary>
    /// 保存螺丝拧紧数据
    /// </summary>
    /// <param name="screw"></param>
    /// <param name="taskId"></param>
    /// <returns></returns>
    public async Task<bool> SaveScrewData(Base_StationTaskScrew screw, int taskId)
    {
        var response = await ApiHelper.SaveScrewData(screw, taskId);
        if (response is { Code: 200 })
        {
            Vm.AddLogMsg.Execute(new LogMessage { Timestamp = DateTime.Now, Content = "保存拧紧数据成功" });
            return true;
        }

        await _mediator.Publish(new AlarmSYSNotification
        {
            Code = AlarmCode.拧紧枪错误, Name = nameof(AlarmCode.拧紧枪错误), Module = AlarmModule.SERVER_MODULE,
            Description = $"保存拧紧数据失败"
        });
        return false;
    }

    /// <summary>
    /// 螺丝拧紧完成
    /// </summary>
    /// <param name="screw"></param>
    /// <returns></returns>
    public async Task<bool> SetScrewTaskFinish(Base_StationTaskScrew screw)
    {
        var response = await ApiHelper.SetScrewTaskFinish(screw);
        if (response.Code == 200)
        {
            Vm.AddLogMsg.Execute(new LogMessage { Timestamp = DateTime.Now, Content = $"【{screw.ScrewSpecs}】拧紧任务完成" });
            return true;
        }

        await _mediator.Publish(new AlarmSYSNotification
        {
            Code = AlarmCode.拧紧枪错误, Name = nameof(AlarmCode.拧紧枪错误), Module = AlarmModule.SERVER_MODULE,
            Description = $"【{screw.ScrewSpecs}】拧紧任务失败"
        });
        return false;
    }
    
    /// <summary>
    /// 保存螺丝反拧数据
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<bool> SaveReverseData(ScrewDataDTO dto)
    {

        var response = await ApiHelper.SaveReverseData(dto);
        if (response != null && response.Code == 200)
        {
            Vm.AddLogMsg.Execute(new LogMessage { Timestamp = DateTime.Now, Content = "保存反拧数据成功" });
            return true;
        }
        else
        {
            await _mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.拧紧枪错误, Name = nameof(AlarmCode.拧紧枪错误), Module = AlarmModule.DESOUTTER_MODULE, Description = $"保存反拧数据失败:{response.Message}" });
            return false;
        }
    }
    
    /// <summary>
    /// 保存螺丝反拧数据
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<bool> SaveTightenByImageData(ScrewDataDTO dto)
    {
        var response = await ApiHelper.SaveTightenByImageData(dto);
        if (response != null && response.Code == 200)
        {
            Vm.AddLogMsg.Execute(new LogMessage { Timestamp = DateTime.Now, Content = "保存图示拧紧数据成功" });
            return true;
        }
        else
        {
            await _mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.拧紧枪错误, Name = nameof(AlarmCode.拧紧枪错误), Module = AlarmModule.DESOUTTER_MODULE, Description = $"保存图示拧紧数据失败:{response.Message}" });
            return false;
        }
    }
    public async Task<bool> SaveNgData(ScrewDataDTO dto)
    {

        var response = await ApiHelper.SaveNgData(dto);
        if (response != null && response.Code == 200)
        {
            Vm.AddLogMsg.Execute(new LogMessage { Timestamp = DateTime.Now, Content = "保存拧紧NG数据成功" });
            return true;
        }
        else
        {
            await _mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.拧紧枪错误, Name = nameof(AlarmCode.拧紧枪错误), Module = AlarmModule.DESOUTTER_MODULE, Description = $"保存拧紧数据失败:{response.Message}" });
            return false;
        }
    }
}