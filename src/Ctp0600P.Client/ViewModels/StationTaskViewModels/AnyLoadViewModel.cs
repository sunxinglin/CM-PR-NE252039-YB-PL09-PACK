using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.Protocols.AnyLoad;

using FutureTech.Mvvm;

using MediatR;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;

namespace Ctp0600P.Client.ViewModels.StationTaskViewModels;

public class AnyLoadViewModel : TaskViewModelBase
{
    public StationTaskDTO StationTaskDTO { get; }
    public Base_StationTaskAnyLoad TaskAnyLoad { get; }
    public int AnyLoadWeightDataTimes;
    public decimal LastWeightData;
    public bool IsDealingWeightData = false;
    public IAnyLoadApi _anyLoadApi { get; }
    
    public AnyLoadViewModel(StationTaskDTO stationTaskDTO, IAnyLoadApi ianyLoadApi, IMediator mediator)
    {
        StationTaskDTO = stationTaskDTO;
        TaskAnyLoad = StationTaskDTO.StationTaskAnyLoad;
        _anyLoadApi = ianyLoadApi;
        this.mediator = mediator;
        this.CatchWeightData = new AsyncRelayCommand<object>(
            obj =>
            {
                try
                {
                    UIHelper.RunInUIThread(pl =>
                    {
                        _anyLoadApi.ReadCurrentWeightData();
                    });
                }
                catch (Exception e)
                {
                }
                return Task.CompletedTask;
            },
            o => true
        );
        this.AnyLoadConfirm = new AsyncRelayCommand<object>(
            obj =>
            {
                try
                {
                    IsDealingWeightData = true;

                    if (WeightData >= TaskAnyLoad.MinWeight && WeightData <= TaskAnyLoad.MaxWeight)
                    {
                        TaskAnyLoad.WeightData = WeightData;
                        TaskAnyLoad.UpMesCode = StationTaskDTO.StationTaskAnyLoad.UpMesCode;
                        WeightData = 0;
                        CompleteTask();
                    }
                    else
                    {
                        var notice = new MessageNotice() { messageType = MessageTypeEnum.警告, showType = MessageShowType.右下角弹窗, MessageStr = "包重不在管控范围内，请重试" };
                        this.mediator.Publish(notice);
                    }
                    IsDealingWeightData = false;
                }
                catch (Exception e)
                {
                }
                return Task.CompletedTask;
            },
            o => true
        );

        MaxWeightData = (decimal)TaskAnyLoad.MaxWeight;
        MinWeightData = (decimal)TaskAnyLoad.MinWeight;

        BindHisData();
    }
    //捕获称重信息
    public ICommand CatchWeightData { get; }
    //称重确认
    public ICommand AnyLoadConfirm { get; }


    public void BindHisData()
    {
        var histDto = App.HisTaskData2.StationTaskRecords.FirstOrDefault(f => f.StationTaskId == StationTaskDTO.StationTaskId);
        var anyload = histDto?.AnyLoads;
        if(anyload != null)
        {
            StationTaskDTO.HasFinish = histDto.StationTaskRecord.Status == StationTaskStatusEnum.已完成;
            TaskAnyLoad.HasPassed = anyload.Status == StationTaskStatusEnum.已完成;
            WeightData = anyload.WeightData;
        }
    }
    /// <summary>
    /// 绑定历史信息
    /// </summary>
    /// <param name="record"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void BindHisData(Proc_StationTask_Record record)
    {
        var hisRecord = record.Proc_StationTask_AnyLoad;
        if (hisRecord == null) return;
        StationTaskDTO.HasFinish = record.Status == StationTaskStatusEnum.已完成;
        TaskAnyLoad.HasPassed = hisRecord.Status == StationTaskStatusEnum.已完成;
        WeightData = hisRecord.WeightData;
    }

    public void CatchAnyLoadMessage(AnyLoadRequest request)
    {
        try
        {
            if (IsDealingWeightData) return;
            var weightData = decimal.Parse(request.AnyLoadContext.Replace("KG", ""));
            WeightData = weightData;
        }
        catch (Exception ex)
        {
            mediator.Publish(new AlarmSYSNotification
            {
                Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.ELECTRONIC_SCALE,
                Description = $"{ex.Message}"
            });
        }
    }


    public void Test()
    {
        WeightData = 110;
        TaskAnyLoad.WeightData = WeightData;

        CompleteTask();
    }

    /// <summary>
    /// 当前任务完成
    /// </summary>
    private async void CompleteTask()
    {
        TaskAnyLoad.HasPassed = true;
        bool saveOK = await SaveAnyLoadData(StationTaskDTO.StationTaskId);
        if (saveOK)
        {
            StationTaskDTO.HasFinish = true;
            OnCompleteTask(StationTaskDTO);
        }
        else
        {
            var notice = new MessageNotice
            {
                messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"保存失败，请联系管理员！"
            };
            await this.mediator.Publish(notice);
            await mediator.Publish(new AlarmSYSNotification
            {
                Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.SERVER_MODULE,
                Description = $"当前任务数据保存失败，请联系管理员！"
            });
        }
    }


    private readonly IMediator mediator;

    private decimal _WeightData;
    public decimal WeightData
    {
        get => _WeightData;
        set
        {
            if (_WeightData != value)
            {
                _WeightData = value;
                OnPropertyChanged(nameof(WeightData));
            }
        }
    }

    private decimal _MaxWeightData;
    public decimal MaxWeightData
    {
        get => _MaxWeightData;
        set
        {
            if (_MaxWeightData != value)
            {
                _MaxWeightData = value;
                OnPropertyChanged(nameof(MaxWeightData));
            }
        }
    }

    private decimal _MinWeightData;
    public decimal MinWeightData
    {
        get => _MinWeightData;
        set
        {
            if (_MinWeightData != value)
            {
                _MinWeightData = value;
                OnPropertyChanged(nameof(MinWeightData));
            }
        }
    }

    public async Task<bool> SaveAnyLoadData(int taskId)
    {
        TaskAnyLoad.CreateUserID = App._RealtimePage.WorkingUserId;
        TaskAnyLoad.CreateTime = DateTime.Now;
        return await App._RealtimePage.SaveAnyLoadData(TaskAnyLoad, taskId);
    }


}