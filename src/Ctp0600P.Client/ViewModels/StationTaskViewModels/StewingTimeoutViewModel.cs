using System;
using System.Windows;

using Ctp0600P.Client.Apis;
using Ctp0600P.Client.Protocols;

using MediatR;

using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;

namespace Ctp0600P.Client.ViewModels.StationTaskViewModels;

public class StewingTimeoutViewModel : TaskViewModelBase
{
    public Base_StationTaskStewingTime TaskStewingTime { get; }
    private readonly APIHelper _apiHelper;
    private readonly IMediator _mediator;

    public StationTaskDTO _StationTaskDTO { get; }

    public StewingTimeoutViewModel(StationTaskDTO stationTaskDTO, APIHelper apiHelper, IMediator mediator)
    {
        _apiHelper = apiHelper;
        _mediator = mediator;
        _StationTaskDTO = stationTaskDTO;
        //TaskStewingTime = stationTaskDTO.StationTaskStewingTime;
        //AllowTime = Math.Round(TaskStewingTime.MinTime, 0).ToString() + "分钟";
        //StepCode = TaskStewingTime.TaskEndStepCode;
        //NowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        BindHisData();

    }

    #region 【绑定历史信息】

    public void BindHisData()
    {
        if (App.HisTaskData != null)
        {
            foreach (var record in App.HisTaskData.Proc_StationTask_Main.Proc_StationTask_Records)
            {
                if (record.Base_StationTaskId == _StationTaskDTO.StationTaskId)
                {
                    BindHisData(record);
                }
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
            //var gluHisData = record.Proc_StationTask_StewingTime;
            //if (gluHisData == null) return;
            //Pass = gluHisData.Pass ? "通过" : "不通过";
            //TaskStewingTime.HasFinish = true;
            //TaskStewingTime.HasPassed = gluHisData.Pass;
            //RealTime = Math.Round(gluHisData.StewingTime, 0).ToString() + "分钟";
        }
        catch (Exception ex)
        {
            _mediator.Publish(new AlarmSYSNotification
            {
                Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.SERVER_MODULE,
                Description = $"绑定静置时长检测历史数据错误:{ex.Message}\r\n{ex.StackTrace}"
            });
        }
    }

    #endregion

    public void CatchStewingTimeMessage()
    {
        if (string.IsNullOrEmpty(Pass))
        {
            Application.Current.Dispatcher.Invoke((Action)(async () =>
            {
                try
                {
                    var allowTime = TaskStewingTime.MinTime;

                    var timeResult = await _apiHelper.GetStepTaskEndTime(App.PackBarCode, StepCode);
                    if (timeResult.Code != 200)
                    {
                        await _mediator.Publish(new AlarmSYSNotification { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"未查询到PACK码：{App.PackBarCode}在工位{StepCode}任务信息" });
                    }
                    else
                    {
                        //TaskStewingTime.HasFinish = true;
                        AllowTime = Math.Round(allowTime, 0) + "分钟";
                        var startTime = timeResult.Result;
                        StewingStartTime = startTime.ToString("yyyy-MM-dd HH:mm:ss");

                        //计算实际时间
                        var time = Math.Round((DateTime.Parse(NowTime) - startTime).TotalMinutes, 0);
                        RealTime = time + "分钟";

                        var checkResult = time >= Convert.ToInt32(allowTime);

                        Pass = checkResult ? "通过" : "不通过";

                        if (!checkResult)
                        {
                            await _mediator.Publish(new AlarmSYSNotification
                            {
                                Code = AlarmCode.静置时长不足报警, Name = nameof(AlarmCode.静置时长不足报警),
                                Module = AlarmModule.SERVER_MODULE,
                                Description = $"静置时长未通过\r\n管控时长{AllowTime}\r\n实际时长{RealTime}"
                            });
                        }

                        TaskStewingTime.HasPassed = checkResult;
                        TaskStewingTime.StewingTime = Convert.ToDecimal(time);
                        TaskStewingTime.StewingCollectTime = DateTime.Now;
                        TaskStewingTime.StewingStartTime = startTime;
                        TaskStewingTime.PackCode = App.PackBarCode;
                        TaskStewingTime.StationId = App._StepStationSetting.Station.Id;
                        TaskStewingTime.StepId = App._StepStationSetting.Step.Id;
                        var saveResult = await _apiHelper.SaveStewingTimeMessage(TaskStewingTime);

                        if (saveResult.Code == 200 && checkResult)
                        {
                            _StationTaskDTO.HasFinish = true;
                            OnCompleteTask(_StationTaskDTO);
                        }
                        else if (saveResult.Code != 200)
                        {
                            await _mediator.Publish(new AlarmSYSNotification
                            {
                                Code = AlarmCode.静置时长不足报警, Name = nameof(AlarmCode.静置时长不足报警),
                                Module = AlarmModule.SERVER_MODULE, Description = $"保存静置时长检测数据失败"
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    await _mediator.Publish(new AlarmSYSNotification
                    {
                        Code = AlarmCode.静置时长不足报警, Name = nameof(AlarmCode.静置时长不足报警),
                        Module = AlarmModule.SERVER_MODULE, Description = $"[静置时长检测]{ex.Message}\r\n{ex.StackTrace}"
                    });
                }
            }));
        }

    }

    #region 【声明】
    private string _stewingStartTime;
    public string StewingStartTime
    {
        get => _stewingStartTime;
        set
        {
            _stewingStartTime = value;
            OnPropertyChanged(nameof(StewingStartTime));
        }
    }

    private string _TimeName;
    public string TimeName
    {
        get => _TimeName;
        set
        {
            _TimeName = value;
            OnPropertyChanged(nameof(TimeName));
        }
    }

    private string nowTime;
    public string NowTime
    {
        get => nowTime;
        set
        {
            nowTime = value;
            OnPropertyChanged(nameof(NowTime));
        }
    }

    private string pass;
    public string Pass
    {
        get => pass;
        set
        {
            pass = value;
            OnPropertyChanged(nameof(Pass));
        }
    }

    private string _AllowTime;
    public string AllowTime
    {
        get => _AllowTime;
        set
        {
            _AllowTime = value;
            OnPropertyChanged(nameof(AllowTime));
        }
    }

    private string _RealTime;
    public string RealTime
    {
        get => _RealTime;
        set
        {
            _RealTime = value;
            OnPropertyChanged(nameof(RealTime));
        }
    }

    private string _StepCode;
    public string StepCode
    {
        get => _StepCode;
        set
        {
            _StepCode = value;
            OnPropertyChanged(nameof(StepCode));
        }
    }
        
    #endregion
}