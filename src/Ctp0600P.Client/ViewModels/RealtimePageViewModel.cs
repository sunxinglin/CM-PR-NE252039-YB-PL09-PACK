using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using AsZero.Core.Services.Messages;
using AsZero.Core.Services.Repos;

using Catl.WebServices.MIFindCustomAndSfcData;

using Ctp0600P.Client.Apis;
using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using Ctp0600P.Client.CommonEntity;
using Ctp0600P.Client.IO;
using Ctp0600P.Client.Notifications;
using Ctp0600P.Client.PLC.Context;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Client.Protocols.AnyLoad;
using Ctp0600P.Client.Protocols.ScanCode.Models;
using Ctp0600P.Client.UserControls.AGV;
using Ctp0600P.Client.UserControls.PLC01;
using Ctp0600P.Client.ViewModels.StationTaskViewModels;
using Ctp0600P.Client.Views.StationTaskPages;
using Ctp0600P.Client.Views.Windows;
using Ctp0600P.Shared;

using FutureTech.Mvvm;

using Itminus.FSharpExtensions;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FSharp.Core;

using Reactive.Bindings;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;
using Yee.Services.CatlMesInvoker;

namespace Ctp0600P.Client.ViewModels;

public class RealtimePageViewModel : ViewModelBase
{
    /// <summary>
    /// 屏幕上的最大日志数量
    /// </summary>
    private const int LOGS_MAX = 100;

    private readonly ILogger<RealtimePageViewModel> _logger;
    private readonly APIHelper _apiHelper;
    private readonly IAnyLoadApi _anyLoadApi;
    private readonly IServiceProvider _sp;
    private readonly IMediator _mediator;
    private readonly ICatlMesInvoker _catlMesInvoker;
    private readonly AppViewModel _appvm;
    private readonly StationPLCContext _stationPLCContext;
    private readonly IOptionsMonitor<StepStationSetting> _stepStationSetting;

    public delegate void NGBack(string packcode, bool ngresult);

    public event NGBack OnBack;

    private readonly SemaphoreSlim _semaphoreSlim = new(1);

    public Subject<bool> TimerWorking = new();

    public ReactiveProperty<PLC01MonitorViewModel> PLC01MonitorViewModel { get; }

    public RealtimePageViewModel(ILogger<RealtimePageViewModel> logger, AppViewModel appvm,
        PLC01MonitorViewModel plc01MonitorViewModel,
        APIHelper apiHelper, IAnyLoadApi anyLoadApi, IServiceProvider service,
        IMediator mediator, ICatlMesInvoker catlMesInvoker, StationPLCContext stationPLCContext,
        IOptionsMonitor<StepStationSetting> stepStationSetting)
    {
        #region 基础初始化和依赖注入
        this._appvm = appvm;
        this._logger = logger;
        this._apiHelper = apiHelper;
        this._anyLoadApi = anyLoadApi;
        this._sp = service;
        this._mediator = mediator;
        this._catlMesInvoker = catlMesInvoker;
        this.PLC01MonitorViewModel = new ReactiveProperty<PLC01MonitorViewModel>(plc01MonitorViewModel);
        this._stationPLCContext = stationPLCContext;
        this._stepStationSetting = stepStationSetting;

        if (App._StepStationSetting.StepType == StepTypeEnum.线外人工站)
        {
            // 线外人工站不需要AGV，赋值0防止空引用
            App.AGVCode = "0";
        }

        // 注册扫码枪事件：只要扫了码，就调用CatchScanCodeMessage方法
        App.ScanCodeGunRequestSubject.ToEvent().OnNext += CatchScanCodeMessage;
        #endregion

        #region 日志/报警

        this.AddLogMsg = new AsyncRelayCommand<LogMessage>(s =>
        {
            try
            {
                UIHelper.RunInUIThread(pl =>
                {
                    var logCount = this.Logs.Count;
                    // 限制日志条数
                    if (logCount >= LOGS_MAX)
                    {
                        this.Logs.RemoveAt(0);
                    }
                    this.Logs.Add(s);
                    this.IsLogsChanged = true;
                });
            }
            catch (Exception e)
            {
                _logger.LogError("添加日志出错：{EMessage}\n{EStackTrace}", e.Message, e.StackTrace);
            }

            return Task.CompletedTask;
        }, o => true);

        this.UpdateAlarms = new AsyncRelayCommand<Alarm>(s =>
        {
            try
            {
                // 处理某个具体的报警
                if (s != null)
                {
                    UIHelper.RunInUIThread(async pl =>
                    {
                        // 在当前报警列表中查找是否存在相同的报警
                        var alarm = this.Alarms
                            .FirstOrDefault(o => o.Code == s.Code && o.Name == s.Name && o.Description == s.Description);
                        if (alarm is { IsFinish: false })
                        {
                            alarm = s;
                        }
                        else if (s.IsFinish && alarm != null)
                        {
                            this.Alarms.Remove(alarm);
                            // 临时解决方案：为了修复服务器断开连接又连上后，系统提示有【当前存在未处理的报警】问题
                            if (this.Alarms.Count <= 0)
                            {
                                App.StationTaskNGPause = false;
                            }

                            var dto = new AlarmDTO
                            {
                                StepCode = App._StepStationSetting.Step?.Code,
                                Name = alarm.Name,
                                Description = alarm.Description,
                                OccurTime = alarm.OccurTime,
                                PackCode = s.PackCode,
                                StationCode = App._StepStationSetting.StationCode,
                                Code = s.Code,
                                Module = s.Module,
                                IsFinish = true
                            };
                            await _apiHelper.OccurAlarm(dto);
                        }
                        else
                        {
                            this.Alarms.Add(s);
                            var ioBusinessProcess = service.GetService<IOBoxBusinessProcess>();
                            ioBusinessProcess.hasDeal = false;
                            App.StationTaskNGPause = true;
                            // 通知PLC报警
                            var manualStationNotifyPLCContext = service.GetService<StationPLCContext>();
                            manualStationNotifyPLCContext.Alarm = true;

                            await _mediator.Publish(new UILogNotification
                            {
                                LogMessage = new LogMessage
                                {
                                    Level = LogLevel.Error,
                                    Content = s.Description,
                                    Timestamp = DateTime.Now
                                }
                            });
                        }

                        this.IsAlarmsChanged = true;
                    });
                }
                // 操作员按复位：清除报警
                else
                {
                    UIHelper.RunInUIThread(async p1 =>
                    {
                        // 构造要清除的报警列表DTO
                        var dtos = new List<AlarmDTO>();
                        var hasTightenNGReset = false;
                        var hasTightenNGResetDeviceNos = new List<string>();
                        // 遍历当前所有报警
                        foreach (var alarm in this.Alarms)
                        {
                            // 转换为DTO
                            var dto = new AlarmDTO
                            {
                                Code = alarm.Code,
                                StepCode = App._StepStationSetting.Step?.Code ?? string.Empty,
                                StationCode = App._StepStationSetting.StationCode,
                                PackCode = alarm.PackCode ?? string.Empty,
                                Name = alarm.Name ?? string.Empty,
                                Module = alarm.Module ?? string.Empty,
                                Description = alarm.Description ?? string.Empty,
                                OccurTime = alarm.OccurTime,
                                DeviceNo = alarm.DeviceNo ?? string.Empty,
                                IsFinish = true
                            };
                            dtos.Add(dto);
                        }

                        if (dtos is { Count: > 0 })
                        {
                            // 判断是否有拧紧NG的复位
                            hasTightenNGReset = this.Alarms.Any(e => e.Name == "拧紧NG");
                            // 记录哪些枪报了错
                            hasTightenNGResetDeviceNos = this.Alarms.Where(e => e.Name == "拧紧NG")
                                .Select(e => e.DeviceNo).ToList();

                            _logger.LogDebug("复位报警信息列表：{Serialize}", JsonSerializer.Serialize(dtos));
                            // 清除所有报警
                            await _apiHelper.ClearALLAlarm(dtos);
                            this.Alarms.Clear();
                        }

                        if (this.Alarms.Count == 0)
                        {
                            // 解除全局暂停
                            App.StationTaskNGPause = false;

                            if (App.ActivePage == null)
                            {
                                return;
                            }
                            
                            // 若操作员清除的是“拧紧NG”，则通知BoltGun上使能
                            if (hasTightenNGReset)
                            {
                                switch (App.ActivePage.GetType().Name)
                                {
                                    case "BoltGun":
                                        await _mediator.Publish(new UILogNotification(new LogMessage
                                        {
                                            Content =
                                                $"[UpdateAlarms]调用了[EnableBoltGuns] for [BoltGun], {true}-{JsonSerializer.Serialize(hasTightenNGResetDeviceNos)}",
                                            Level = LogLevel.Trace, Timestamp = DateTime.Now
                                        }));
                                        ((BoltGun)App.ActivePage)._VM.EnableBoltGuns(true, hasTightenNGResetDeviceNos.ToArray());
                                        break;
                                    case "RepairBoltGunCommon":
                                        await _mediator.Publish(new UILogNotification(new LogMessage
                                        {
                                            Content = "[UpdateAlarms]调用了[EnableBoltGuns] for [RepairBoltGunCommon]",
                                            Level = LogLevel.Trace, Timestamp = DateTime.Now
                                        }));
                                        ((RepairBoltGunCommon)App.ActivePage)._VM.EnableBoltGuns();
                                        break;
                                    // case "TightenByImage":
                                    //     ((TightenByImage)App.ActivePage)._VM.ReverseScrew();
                                    //     break;
                                }
                            }
                        }
                    });
                }
            }
            catch (Exception e)
            {
                _logger.LogError("更新警报信息出错：{EMessage}\n{EStackTrace}", e.Message, e.StackTrace);
            }

            return Task.CompletedTask;
        }, o => true);

        this.DeleteAlarm = new AsyncRelayCommand<Alarm>(s =>
        {
            try
            {
                if (s != null)
                {
                    UIHelper.RunInUIThread(pl =>
                    {
                        var alarm = this.Alarms.FirstOrDefault(o => o.Code == s.Code && o.Name == s.Name);
                        if (alarm is { IsFinish: false })
                        {
                            this.Alarms.Remove(alarm);
                            this.IsAlarmsChanged = true;
                        }
                    });
                }
            }
            catch (Exception e)
            {
                _logger.LogError("更新警报信息出错：{EMessage}\n{EStackTrace}", e.Message, e.StackTrace);
            }

            return Task.CompletedTask;
        }, o => true);

        #endregion

        #region AGV绑定

        this.Bingagv = new AsyncRelayCommand<object>(async s =>
        {
            try
            {
                if (App._RealtimePage == null)
                {
                    return;
                }

                if (!string.IsNullOrEmpty(App.PackBarCode))
                {
                    var notice = new MessageNotice
                    {
                        messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗,
                        MessageStr = $"当前有进行中的任务，请注销后重试"
                    };
                    await _mediator.Publish(notice);
                    return;
                }

                if (string.IsNullOrEmpty(App.AGVCode))
                {
                    var notice = new MessageNotice
                    {
                        messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗,
                        MessageStr = $"小车未进站，请小车进站后重试或通过网页操作"
                    };
                    await _mediator.Publish(notice);
                    return;
                }

                using (var scope = service.CreateScope())
                {
                    var sp = scope.ServiceProvider;

                    var winCheck = sp.GetRequiredService<CheckPowerPage>();
                    if (winCheck.Visibility != Visibility.Visible)
                    {
                        ((CheckPowerViewModel)winCheck.DataContext).Action = delegate
                        {
                            winCheck.Close();
                            winCheck.Visibility = Visibility.Hidden;
                            var win = sp.GetRequiredService<AGVBingPage>();
                            App.BingAgvWinOpen = true;
                            App._ActivityAGVBingPage = win;
                            win.Show();
                        };
                        ((CheckPowerViewModel)winCheck.DataContext).ModuleName = "AGV";
                        winCheck.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                var notice = new MessageNotice
                {
                    messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"打开窗口错误,请联系管理员！"
                };
                await _mediator.Publish(notice);
            }
        }, o => true);

        #endregion

        #region 初始化界面

        this.InitPage = new AsyncRelayCommand<object>(s =>
        {
            var confirmResult = MessageBox.Show("确认要初始化客户端吗？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirmResult == MessageBoxResult.Yes)
            {
                InitEmpty_TaskData(true);
            }

            return Task.CompletedTask;
        }, o => true);

        #endregion

        #region 跳步
        this.SkipStep = new AsyncRelayCommand<object>(async s =>
        {
            try
            {
                if (App.HisTaskData2 == null)
                {
                    await _mediator.Publish(new AlarmSYSNotification
                    {
                        Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误),
                        Module = AlarmModule.SERVER_MODULE, Description = "没有进行中的任务,无法跳步"
                    });
                    return;
                }

                using (var scope = service.CreateScope())
                {
                    var sp = scope.ServiceProvider;

                    var wincheck = sp.GetRequiredService<CheckPowerPage>();
                    ((CheckPowerViewModel)wincheck.DataContext).Action = async delegate
                    {
                        wincheck.Close();
                        if (App._StepStationSetting.IsDebug)
                        {
                            var win = new SkipStepWindow(new SkipStepViewModel(
                                StationTaskDataList.Select(s => s.StationTaskDTO.StationTask).ToList(), _apiHelper,
                                this, _mediator));
                            win.Show();
                            return;
                        }

                        await SkipStepMess();
                    };
                    ((CheckPowerViewModel)wincheck.DataContext).ModuleName = "跳步";
                    wincheck.ShowDialog();
                }
            }
            catch
            {
                await _mediator.Publish(new AlarmSYSNotification
                    { Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Description = $"设置跳步错误" });
            }
        }, o => true);

        #endregion

        #region 数据重传

        this.UploadAgain = new AsyncRelayCommand<object>(async s =>
        {
            try
            {
                if (!string.IsNullOrEmpty(App.PackBarCode))
                {
                    await _mediator.Publish(new AlarmSYSNotification
                    {
                        Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误),
                        Description = $"当前存在工作中的流程，不允许使用数据重传！"
                    });
                    return;
                }

                var win = new UploadAgain(new UploadAgainViewModel(service));
                App.UploadAgainWin = win;
                win.Show();
            }
            catch (Exception ex)
            {
                await _mediator.Publish(new AlarmSYSNotification
                    { Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Description = $"数据重传页面打开失败！" });
            }
        }, o => true);

        #endregion
    }

    private async Task SkipStepMess()
    {
        var CurTaskTask = StationTaskDataList.FirstOrDefault(f => f.StepNo == CurStepNo);
        if (CurTaskTask == null)
        {
            return;
        }

        var dto = CurTaskTask.StationTaskDTO;
        if (dto.StationTaskScanAccountCard != null)
        {
            await _mediator.Publish(new AlarmSYSNotification
            {
                Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.SERVER_MODULE,
                Description = "扫描员工卡任务类型无法跳过"
            });
            return;
        }

        var result = await _apiHelper.MakeTaskComplateForce(dto, App.HisTaskData2.MainId);
        if (result.Code != 200)
        {
            await _mediator.Publish(new AlarmSYSNotification
            {
                Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.SERVER_MODULE,
                Description = $"{result.Message}"
            });
            return;
        }

        SetStationCurTaskFinish(dto);
        Application.Current.Dispatcher.Invoke(() =>
        {
            this.GoNextUndoTask();
            this.DealTaskStep();
        });
    }

    /// <summary>
    /// 根据任务类型和数据映射到对应的任务页面
    /// </summary>
    /// <param name="type">任务类型</param>
    /// <param name="mapping">任务数据映射</param>
    /// <param name="taskBomList">任务BOM列表</param>
    /// <param name="taskScrew">任务螺丝列表</param>
    /// <param name="screwCountBeforeCurrentTask">当前任务之前的螺丝数量</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private Task<Page> MapSourceToTaskPage(StationTaskTypeEnum type, StationTaskDTO mapping,
        List<Base_StationTaskBom> taskBomList, ObservableCollection<Base_StationTaskScrew> taskScrew,
        int screwCountBeforeCurrentTask)
    {
        try
        {
            switch (type)
            {
                case StationTaskTypeEnum.扫码:
                    return Task.FromResult<Page>(new ScanCode(new ScanCodeViewModel(mapping, taskBomList.First(),
                        _mediator, _catlMesInvoker, _apiHelper)));
                case StationTaskTypeEnum.扫描员工卡:
                    return Task.FromResult<Page>(
                        new ScanAccountCard(new ScanAccountCardViewModel(mapping, _apiHelper, _mediator)));
                case StationTaskTypeEnum.超时检测:
                    return Task.FromResult<Page>(
                        new CheckTimeout(new CheckTimeoutViewModel(mapping, _apiHelper, _mediator)));
                case StationTaskTypeEnum.人工拧螺丝:
                    return Task.FromResult<Page>(new BoltGun(new BoltGunViewModel(mapping, taskScrew, _mediator, _sp,
                        _apiHelper, _stationPLCContext, screwCountBeforeCurrentTask)));
                case StationTaskTypeEnum.称重:
                    return Task.FromResult<Page>(new AnyLoad(new AnyLoadViewModel(mapping, _anyLoadApi, _mediator)));
                case StationTaskTypeEnum.放行:
                    return Task.FromResult<Page>(new LetGoPage(mapping, _apiHelper, _mediator, _stationPLCContext));
                case StationTaskTypeEnum.补拧:
                    return Task.FromResult<Page>(new RepairBoltGunCommon(
                        new RepairBoltGun_CommonViewModel(mapping, _sp, _apiHelper, _mediator, _stationPLCContext,
                            _stepStationSetting), _sp, _mediator));
                case StationTaskTypeEnum.用户输入:
                    return Task.FromResult<Page>(
                        new UserInputCollect(new UserInputCollectViewModel(mapping, _mediator, _apiHelper)));
                case StationTaskTypeEnum.扫码输入:
                    return Task.FromResult<Page>(
                        new ScanCollect(new ScanCollectViewModel(mapping, _mediator, _apiHelper)));
                case StationTaskTypeEnum.时间记录:
                    return Task.FromResult<Page>(
                        new RecordTimeTaskPage(new RecordTimeViewModel(mapping, _apiHelper, _mediator)));
                case StationTaskTypeEnum.图示拧紧:
                    return Task.FromResult<Page>(new TightenByImage(
                        new TightenByImageViewModel(_apiHelper, _mediator, _sp, mapping, _stationPLCContext)));

                default:
                    throw new Exception($"未知的任务类型={type}");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return Task.FromResult<Page>(null);
        }
    }

    /// <summary>
    /// 处理复位报警
    /// </summary>
    internal void DealResetAlarm()
    {
        using var scope = _sp.CreateScope();
        var sp = scope.ServiceProvider;
        var boltGunNGs = Alarms.Where(d => d.Code == AlarmCode.拧紧枪错误 && d.Name == nameof(AlarmCode.拧紧NG)).ToList();
        if (boltGunNGs is { Count: > 0 })
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var winCheck = sp.GetRequiredService<CheckPowerPage>();
                var vm = (CheckPowerViewModel)winCheck.DataContext;
                vm.Action = async delegate
                {
                    //可在此处调用接口来判断相关逻辑（例如：NG次数上限、角色解锁次数）
                    if (boltGunNGs.FirstOrDefault()?.TightenNGExtra != null)
                    {
                        var checkResult = await _apiHelper.SaveScrewNGResetRecord(new Proc_ScrewNGResetRecordSaveDTO
                        {
                            Account = App.PowerUser,
                            StationCode = App._StepStationSetting.StationCode,
                            PackCode = App.PackBarCode,
                            ScrewSerialNo = boltGunNGs.FirstOrDefault()?.TightenNGExtra?.ScrewSerialNo ?? 0,
                        });
                        if (checkResult.Code != 200)
                        {
                            await _mediator.Publish(new UILogNotification(new LogMessage
                            {
                                Content = $"{checkResult.Message}",
                                Level = LogLevel.Information,
                                Timestamp = DateTime.Now
                            }));

                            vm.ErrorMessage = $"{checkResult.Message}";

                            return;
                        }
                    }

                    winCheck.Close();
                    UpdateAlarms?.Execute(null);
                };
                vm.ModuleName = "拧紧NG复位";
                winCheck.ShowDialog();
            });
        }
        else
        {
            UpdateAlarms?.Execute(null);
        }
    }

    /// <summary>
    /// 检测当前工位任务是否完成
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public bool CheckStationTaskFinish(StationTaskDTO dto)
    {
        var lastSequence = StationTaskDataList.OrderBy(i => i.StepNo).Last();
        if (lastSequence.StepNo == dto.Sequence)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 设置当前任务完成
    /// </summary>
    /// <param name="dto"></param>
    public void SetStationCurTaskFinish(StationTaskDTO dto)
    {
        var task = StationTaskDataList.FirstOrDefault(t => t.StationTaskDTO == dto);
        task.HasFinish = true;
    }

    /// <summary>
    /// 扫描PACK码后，加载工位任务配方数据
    /// </summary>
    private async void InitVMData(IList<StationTaskDTO> stationTaskList = null)
    {
        await InitVMData_LeftTag(stationTaskList);
        await InitVMData_RightContent(stationTaskList);
    }

    /// <summary>
    /// 初始化右侧内容区域的任务数据
    /// </summary>
    /// <param name="stationTaskList"></param>
    private async Task InitVMData_RightContent(IList<StationTaskDTO> stationTaskList = null)
    {
        try
        {
            StationTaskDataList = new ObservableCollection<StationTaskData>();
            foreach (var mapping in stationTaskList)
            {
                var screwCountBeforeCurrentTask = GetScrewCountBeforeCurrentTask(stationTaskList, mapping);
                await DoViewRightData(mapping, screwCountBeforeCurrentTask);
            }

            // 隐藏最后一个步骤
            var lastSequence = StationTaskLeftTagDataList.OrderBy(i => i.StepNo).Last();
            lastSequence.NeedShow = Visibility.Collapsed;
            CurStepNo = 1;
        }
        catch (Exception ex)
        {
            await _mediator.Publish(new AlarmSYSNotification
            {
                Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.SERVER_MODULE,
                Description = $"工位任务配方数据错误：{ex.Message}\r\n{ex.StackTrace}"
            });
        }
    }

    /// <summary>
    /// 获取当前拧紧任务之前所有拧紧任务的螺丝数量总和
    /// </summary>
    /// <param name="stationTaskList"></param>
    /// <param name="currentTask"></param>
    /// <returns></returns>
    private int GetScrewCountBeforeCurrentTask(IList<StationTaskDTO> stationTaskList, StationTaskDTO currentTask)
    {
        try
        {
            //非拧紧任务，跳过
            if (currentTask.StationTask.Type != StationTaskTypeEnum.人工拧螺丝)
            {
                return 0;
            }

            //筛选出当前任务之前的所有拧紧任务
            var screwTaskList = stationTaskList.Where(e =>
                e.StationTask.Type == StationTaskTypeEnum.人工拧螺丝 && e.Sequence < currentTask.Sequence).ToList();

            //计算螺丝总数
            var screwCountBeforeCurrentTask = 0;
            foreach (var screwTask in screwTaskList)
            {
                screwCountBeforeCurrentTask += screwTask.StationTaskScrew?.Sum(e => e.UseNum) ?? 0;
            }

            return screwCountBeforeCurrentTask;
        }
        catch (Exception e)
        {
            _logger.LogError("更新警报信息出错：{EMessage}\n{EStackTrace}", e.Message, e.StackTrace);
            return 0;
        }
    }

    private async Task DoViewRightData(StationTaskDTO mapping, int screwCountBeforeCurrentTask)
    {
        var hisTaskRecord = new RecordHistoryDTO();
        var hisStatus = StationTaskStatusEnum.未开始;
        if (App.HisTaskData2 != null)
        {
            // 查找相同任务且已完成的对象，然后将状态改为已完成
            hisTaskRecord = App.HisTaskData2.StationTaskRecords.FirstOrDefault(r =>
                r.StationTaskRecord.Base_StationTaskId == mapping.StationTaskId &&
                r.StationTaskRecord.Status == StationTaskStatusEnum.已完成);
            if (hisTaskRecord != null)
            {
                hisStatus = hisTaskRecord.StationTaskRecord.Status;
            }
        }
        
        var taskScrew = mapping.StationTaskScrew == null
            ? new ObservableCollection<Base_StationTaskScrew>() // 非拧紧任务：赋值空集合
            : new ObservableCollection<Base_StationTaskScrew>(mapping.StationTaskScrew);    // 拧紧任务：赋值到可观察对象集合
        var taskBom = mapping.StationTaskBom == null ? new Base_StationTaskBom() : mapping.StationTaskBom.First();
        // 创建任务数据对象并添加到列表
        StationTaskDataList.Add(new StationTaskData
        {
            Visibility = Visibility.Hidden,
            Header = mapping.StationTask.Name,
            StationTaskDTO = mapping,
            StepNo = mapping.Sequence,
            StationTaskBom = taskBom,
            StationTaskScrewList = taskScrew,
            HasFinish = hisStatus == StationTaskStatusEnum.已完成,
            Content = new Frame
            {
                Content = await MapSourceToTaskPage(mapping.StationTask.Type, mapping,
                    new List<Base_StationTaskBom> { taskBom }, taskScrew, screwCountBeforeCurrentTask)
            }
        });
    }

    /// <summary>
    /// 初始化左侧任务列表的标签
    /// </summary>
    /// <param name="stationTaskList"></param>
    /// <returns></returns>
    private async Task InitVMData_LeftTag(IList<StationTaskDTO> stationTaskList = null)
    {
        try
        {
            StationTaskLeftTagDataList =
                new ObservableCollection<StationTaskLeftTagData>(stationTaskList.Select(DoViewLeftData));
            if (StationTaskLeftTagDataList.Count == 0)
            {
                var notice = new MessageNotice
                {
                    messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗,
                    MessageStr = "当前工位任务为空，请检查任务配置！"
                };
                await _mediator.Publish(notice);
                await _mediator.Publish(new AlarmSYSNotification
                {
                    Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.SERVER_MODULE,
                    Description = "当前工位任务为空，请检查任务配置！"
                });
            }
        }
        catch (Exception ex)
        {
            await _mediator.Publish(new AlarmSYSNotification
            {
                Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.SERVER_MODULE,
                Description = $"工位任务配方数据错误：{ex.Message}\r\n{ex.StackTrace}"
            });
        }
    }

    /// <summary>
    /// 生成左侧任务列表的标签数据
    /// </summary>
    /// <param name="mapping">当前要处理的任务配置对象</param>
    /// <returns></returns>
    private StationTaskLeftTagData DoViewLeftData(StationTaskDTO mapping)
    {
        var hisTaskRecord = new RecordHistoryDTO();
        var hisStatus = StationTaskStatusEnum.未开始;
        if (App.HisTaskData2 != null)
        {
            // 查找相同任务且已完成的对象，然后将状态改为已完成
            hisTaskRecord = App.HisTaskData2.StationTaskRecords.FirstOrDefault(r =>
                r.StationTaskRecord.Base_StationTaskId == mapping.StationTaskId &&
                r.StationTaskRecord.Status == StationTaskStatusEnum.已完成);
            if (hisTaskRecord != null)
            {
                hisStatus = hisTaskRecord.StationTaskRecord.Status;
            }
        }
        
        // 创建显示数据对象
        return new StationTaskLeftTagData
        {
            Header = mapping.Sequence + "、" + mapping.StationTask.Name,
            StepNo = mapping.Sequence,
            Status = (int)hisStatus,
            BackBrush = hisStatus switch
            {
                StationTaskStatusEnum.未开始 => Brushes.WhiteSmoke,
                StationTaskStatusEnum.进行中 => Brushes.LightSkyBlue,
                _ => Brushes.LightGreen
            }
        };
    }

    /// <summary>
    /// 初始化界面
    /// </summary>
    public void InitEmpty()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            App.AGVCode = string.Empty;
            InitPageV();
        });
    }

    private void StopTimer()
    {
        this.TimerWorking.OnNext(false);
    }

    /// <summary>
    /// 初始化任务数据
    /// </summary>
    public void InitEmpty_TaskData(bool reloadpeifang)
    {
        Application.Current.Dispatcher.Invoke(InitPageV);
        GC.Collect();
    }

    /// <summary>
    /// 左侧任务标签
    /// </summary>
    public ObservableCollection<StationTaskLeftTagData> StationTaskLeftTagDataList
    {
        get => _StationTaskLeftTagDataList;
        set
        {
            //if (_StationTaskLeftTagDataList != value)
            //{
            _StationTaskLeftTagDataList = value;
            OnPropertyChanged(nameof(StationTaskLeftTagDataList));
            //}
        }
    }

    private ObservableCollection<StationTaskLeftTagData> _StationTaskLeftTagDataList = new();

    /// <summary>
    /// 返工
    /// </summary>
    public void ReWork()
    {
        if (this.StationTaskDataList == null || this.StationTaskDataList.Count == 0)
        {
            _mediator.Publish(new AlarmSYSNotification
            {
                Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.SERVER_MODULE,
                Description = $"没有可以返工的项！"
            });

            return;
        }

        using (var scope = _sp.CreateScope())
        {
            var sp = scope.ServiceProvider;

            var wincheck = sp.GetRequiredService<CheckPowerPage>();
            ((CheckPowerViewModel)wincheck.DataContext).Action = delegate
            {
                try
                {
                    wincheck.Close();
                    var rework = new ReWorkPage(new ReWorkPageViewModel(_mediator, _sp, _apiHelper));
                    rework.ShowDialog();
                    if (HasReWork)
                    {
                        GoNextUndoTask();
                        DealTaskStep();
                        HasReWork = false;
                    }
                }
                catch (Exception ex)
                {
                    _mediator.Publish(new AlarmSYSNotification
                    {
                        Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误),
                        Module = AlarmModule.DESOUTTER_MODULE, Description = $"返工错误：{ex.Message}\r\n{ex.StackTrace}"
                    });
                    throw;
                }
            };
            ((CheckPowerViewModel)wincheck.DataContext).ModuleName = "返工设置";
            wincheck.ShowDialog();
        }
    }

    public void UnBingAGVPack()
    {
        using var scope = _sp.CreateScope();
        var sp = scope.ServiceProvider;

        var wincheck = sp.GetRequiredService<CheckPowerPage>();
        ((CheckPowerViewModel)wincheck.DataContext).Action = delegate
        {
            wincheck.Close();

            _mediator.Publish(new UILogNotification
            {
                LogMessage = new LogMessage
                {
                    Content = "暂不提供解绑功能，由AGV自行解绑",
                    Level = LogLevel.Information,
                    Timestamp = DateTime.Now,
                }
            });
        };
        ((CheckPowerViewModel)wincheck.DataContext).ModuleName = "AGV";
        wincheck.OpenCheck();
    }

    public Response DealRunAGV()
    {
        if (App._StepStationSetting.IsDebug)
        {
            return new Response();
        }

        _stationPLCContext.ReleaseAGVReqs.Add(new StationPLCContext.ReleaseAGVReq());

        return new Response { Code = 200 };
    }

    public Action<int> LetScrollBarGo { get; set; }

    /// <summary>
    /// 跳转到下一个未完成任务
    /// </summary>
    public void GoNextUndoTask()
    {
        foreach (var task in StationTaskDataList)
        {
            if (!task.HasFinish)
            {
                CurStepNo = task.StepNo;
                App.CurTaskName = task.StepNo + "、" + task.Header;
                if (CurStepNo > 0)
                {
                    LetScrollBarGo(CurStepNo - 1);
                }

                TaskInitial();

                break;
            }
        }
    }

    /// <summary>
    /// 初始化当前步骤的硬件环境
    /// </summary>
    private async void TaskInitial()
    {
        try
        {
            var stationTaskData = StationTaskDataList[CurStepNo - 1];
            if (stationTaskData == null)
            {
                return;
            }

            var page = (Page)((Frame)stationTaskData.Content).Content;

            switch (page)
            {
                case BoltGun boltGun:
                    boltGun._VM.EnableBoltGuns();
                    break;
                case RepairBoltGunCommon repairBoltGunCommon:
                    repairBoltGunCommon._VM.EnableBoltGuns();
                    break;
            }
        }
        catch (Exception ex)
        {
            var notice = new MessageNotice
                { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = "打开窗口错误,请联系管理员!！" };
            await _mediator.Publish(notice);
            _logger.LogError("打开绑定窗口出错：{ExMessage}\n{ExStackTrace}", ex.Message, ex.StackTrace);
        }
    }

    public void CatchAgvMessage(AgvMsgNotification notification)
    {
        if (notification.Action is AgvActionEnum.进站 or AgvActionEnum.绑码)
        {
            App.AGVCode = notification.AgvNo.ToString();
            App.PackOutCode = notification.PackOutCode;
        }
        else if (notification.Action == AgvActionEnum.离站)
        {
            App.AGVCode = "";
            App.PackOutCode = "";
        }

        AddLogMsg.Execute(new LogMessage
            { Timestamp = DateTime.Now, Content = $"【AGV】{notification.AgvNo}号车[{notification.Action}]，[服务端操作通知]" });
    }

    public async void CatchScanCodeMessage(ScanCodeGunRequest request)
    {
        if (App._RealtimePage == null || !string.IsNullOrEmpty(App.PackBarCode) || App.UploadAgainWin != null ||
            App._ActivityAGVBingPage != null)
        {
            return;
        }

        try
        {
            if (!await _semaphoreSlim.WaitAsync(20))
            {
                return;
            }

            var agvHasInSite = await CheckHasVectorInSite();
            if (agvHasInSite.IsError)
            {
                await _mediator.Publish(new AlarmSYSNotification
                {
                    Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.AGV_MODULE,
                    Description = $"{agvHasInSite.ErrorValue}"
                });
                return;
            }

            if (string.IsNullOrEmpty(App.PackOutCode) && App._StepStationSetting.IsNeedBindOuterCode)
            {
                // C公司出货码为12位，先写死，暂不做兼容
                if (request.ScanCodeContext.Length != 12)
                {
                    await _mediator.Publish(new AlarmSYSNotification
                    {
                        Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误),
                        Module = AlarmModule.SERVER_MODULE, Description = "未扫出货码，请先扫描12位出货码"
                    });
                    return;
                }

                App.PackOutCode = request.ScanCodeContext;
                return;
            }

            //判断是否需要进站。返工过来的扫码请求不进站
            var needStart = true;
            if (request.FromRework)
            {
                needStart = false;
            }

     

            var
                q = //from isQueue in CheckSfcHasWorking(request.ScanCodeContext, App._StepStationSetting.StationCode)//判断条码是否需要进站
                    from ProductPN in
                        GetProductPN(request.ScanCodeContext, App._StepStationSetting.StationCode) //获取产品PN
                    from bindAGV in DealBindAGV(App.AGVCode, request.ScanCodeContext, App.PackOutCode,
                        App._StepStationSetting.StationCode) //绑定AGV
                    from checkBind in CheckVectorBindAndFlowOrder(request.ScanCodeContext, agvHasInSite.ResultValue,
                        App._StepStationSetting.StationCode, ProductPN, App.PackOutCode) //校验AGV绑定与工序逻辑
                    from dealResult in DealMesInSite(request.ScanCodeContext, App._StepStationSetting.StationCode,
                        needStart) //处理进站
                    from Formula in _apiHelper.LoadFormula(App._StepStationSetting.StationCode, ProductPN) //加载产品配方
                    from main in CreateOrLoadTraceInfo(request.ScanCodeContext, App._StepStationSetting.StationCode,
                        ProductPN) //创建生产主记录或加载历史数据
                    from page in LoadPages(request, main, Formula.StationTaskList) //加载界面
                    select ProductPN;

            var r = await q;
            if (r.IsError)
            {
                await _mediator.Publish(new AlarmSYSNotification
                {
                    Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.SERVER_MODULE,
                    Description = $"{r.ErrorValue}"
                });
                return;
            }

            _appvm.CurrentStation.Value = App._StepStationSetting.StationCode;
            App.CurrentStationCode = App._StepStationSetting.StationCode;
            if (request.ScanCodeContext != LastPackCode)
            {
                this.CurrentPackUsedTime = 0;
                LastPackCode = request.ScanCodeContext;
                _stationPLCContext.OverrunAlarm = false;
            }

            this.TimerWorking.OnNext(true);
        }
        catch (Exception ex)
        {
            var notice = new MessageNotice
                { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"{ex.Message}！" };
            await _mediator.Publish(notice);
            await _mediator.Publish(new AlarmSYSNotification
            {
                Code = AlarmCode.系统运行错误, Name = nameof(AlarmCode.系统运行错误), Module = AlarmModule.SERVER_MODULE,
                Description = $"{ex.Message}\r\n{ex.StackTrace}"
            });
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    private async Task<FSharpResult<int, string>> CheckHasVectorInSite()
    {
        try
        {
            if (App._StepStationSetting.StepType == StepTypeEnum.线外人工站 || App._StepStationSetting.IsDebug)
            {
                return 0.ToOkResult<int, string>();
            }

            if (string.IsNullOrEmpty(App.AGVCode))
            {
                return "小车未进站，无法触发工作任务".ToErrResult<int, string>();
            }

            return Convert.ToInt32(App.AGVCode).ToOkResult<int, string>();
        }
        catch (Exception ex)
        {
            return ex.Message.ToErrResult<int, string>();
        }
    }

    private async Task<FSharpResult<string, string>> DealMesInSite(string sfc, string stationCode, bool isNew)
    {
        if (App._StepStationSetting.IsDebug)
        {
            return "00000".ToOkResult<string, string>();
        }

        //isnew为true，按照工位配置来，否则mode_none
        var result =
            await _catlMesInvoker.MiFindCustomAndSfcData(sfc, (isNew ? null : modeProcessSFC.MODE_NONE), stationCode);
        if (result.code != 0)
        {
            return $"(Code={result.code}) {result.message}".ToErrResult<string, string>();
        }
        return result.BarCode_GoodsPN.ToOkResult<string, string>();
    }

    private async Task<FSharpResult<string, string>> GetProductPN(string sfc, string stationCode)
    {
        if (App._StepStationSetting.IsDebug)
        {
            return "00000".ToOkResult<string, string>();
            /*
            // 如果你希望在调试模式下比对Pack码规则，请恢复这段代码 
            var resp = await _apiHelper.GetProductPnFormDb(sfc);
            if (resp.Code != 200)
            {
                return resp.Message.ToErrResult<string, string>();
            }

            if (string.IsNullOrWhiteSpace(resp.Result))
            {
                return "未找到当前Pack码匹配的产品条码规则".ToErrResult<string, string>();
            }

            return resp.Result.ToOkResult<string, string>(); 
            */
        }

        var result = await _catlMesInvoker.MiFindCustomAndSfcData(sfc, modeProcessSFC.MODE_NONE, stationCode);
        if (result.code != 0)
            return $"(Code={result.code}) {result.message}".ToErrResult<string, string>();
        return result.BarCode_GoodsPN.ToOkResult<string, string>();
    }

    private async Task<FSharpResult<bool, string>> CheckSfcHasWorking(string sfc, string stationCode)
    {
        var result = await _catlMesInvoker.CheckSfcStatu(sfc, stationCode);
        return (result.code == 0).ToOkResult<bool, string>();
    }

    private async Task<FSharpResult<StationTaskHistoryDTO, string>> CreateOrLoadTraceInfo(string packCode,
        string stationCode, string productCode)
    {
        return await _apiHelper.CreateOrLoadTraceInfo(packCode, stationCode, productCode);
    }

    private async Task<FSharpResult<string, string>> LoadPages(ScanCodeGunRequest request, 
        StationTaskHistoryDTO HisDto, IList<StationTaskDTO> taskDto)
    {
        App.HisTaskData2 = HisDto;
        ViewRealTimeWork(request, HisDto, taskDto);
        return "ok".ToOkResult<string, string>();
    }

    private async Task<FSharpResult<bool, string>> CheckVectorBindAndFlowOrder(string packCode, int vectorCode,
        string stationCode, string productCode, string packOutCode)
    {
        if (App._StepStationSetting.IsDebug)
        {
            return true.ToOkResult<bool, string>();
        }

        if (string.IsNullOrWhiteSpace(productCode))
        {
            return "未获取到产品PN".ToErrResult<bool, string>();
        }

        if (App._StepStationSetting.StepType != StepTypeEnum.线内人工站)
        {
            return await _apiHelper.CheckFlowOrder(packCode, stationCode, productCode);
        }

        return await _apiHelper.CheckVectorBindAndFlowOrder(packCode, vectorCode, stationCode,
            App._StepStationSetting.StationCode, productCode, packOutCode);
    }


    private async Task<FSharpResult<bool, string>> DealBindAGV(string agvNo, string packCode, 
        string holderBarcode, string stationCode)
    {
        //获取配置确认当前工位是否需要绑定AGV
        if (App._StepStationSetting.IsNeedBind)
        {
            if (string.IsNullOrWhiteSpace(agvNo))
            {
                await _mediator.Publish(new UILogNotification
                {
                    LogMessage = new LogMessage
                    {
                        Timestamp = DateTime.Now,
                        Content = $"AGV未进站，{JsonSerializer.Serialize(_stationPLCContext.AGVBindPackReq)}",
                        Level = LogLevel.Information
                    }
                });
                return "AGV未进站".ToErrResult<bool, string>();
            }
            if (_stationPLCContext.AGVBindPackReq.Req)
            {
                await _mediator.Publish(new UILogNotification
                {
                    LogMessage = new LogMessage
                    {
                        Timestamp = DateTime.Now,
                        Content = $"【AGV绑定】存在处理中的绑定请求，{JsonSerializer.Serialize(_stationPLCContext.AGVBindPackReq)}",
                        Level = LogLevel.Information
                    }
                });
                return "存在处理中的绑定请求".ToErrResult<bool, string>();
            }

            _stationPLCContext.AGVBindPackReq.Req = true;
            _stationPLCContext.AGVBindPackReq.Behavior = 1; //绑定
            _stationPLCContext.AGVBindPackReq.AGVNo = Convert.ToUInt16(agvNo);
            _stationPLCContext.AGVBindPackReq.PackCode = packCode;
            _stationPLCContext.AGVBindPackReq.HolderBarcode = holderBarcode;
            _stationPLCContext.AGVBindPackReq.StationCode = stationCode;
        }

        return true.ToOkResult<bool, string>();
    }

    /// <summary>
    /// 在界面上显示实时工作任务
    /// </summary>
    /// <param name="request">扫码枪请求对象</param>
    /// <param name="hisData">工位历史数据</param>
    /// <param name="taskDTOs">工位任务配方</param>
    private void ViewRealTimeWork(ScanCodeGunRequest request, StationTaskHistoryDTO hisData,
        IList<StationTaskDTO> taskDTOs)
    {
        Application.Current.Dispatcher.Invoke(async () =>
        {
            if (hisData.StationTaskMain is { Status: StationTaskStatusEnum.已完成 })
            {
                await _mediator.Publish(new AlarmSYSNotification
                {
                    Code = AlarmCode.防呆报警, Name = nameof(AlarmCode.防呆报警), Module = AlarmModule.SERVER_MODULE,
                    Description = $"Pack-{request.ScanCodeContext}在工位{App._StepStationSetting.StationCode}已完工"
                });
                return;
            }

            App.PackBarCode = request.ScanCodeContext;
            InitVMData(taskDTOs);

            if (taskDTOs[0].StationTask.Type == StationTaskTypeEnum.扫描员工卡)
            {
                CurStepNo = 1;
            }
            else
            {
                CurStepNo = hisData.StationTaskMain.CurStepNo == 0 ? 1 : hisData.StationTaskMain.CurStepNo;
            }

            DealTaskStep();
            App.CurTaskName = CurStepNo + "、" + taskDTOs[CurStepNo - 1].StationTask.Name;
        });
    }

    /// <summary>
    /// 处理左侧任务标签状态
    /// </summary>
    public void DealTaskStep()
    {
        // 1. 设置当前步骤状态为 "进行中" (Status=1)
        var curTaskTags = StationTaskLeftTagDataList.Where(s => s.StepNo == CurStepNo);
        foreach (var tag in curTaskTags)
        {
            tag.Status = 1;
        }
        // 2. 设置之前步骤状态为 "已完成" (Status=2)
        var prevTaskTags = StationTaskLeftTagDataList.Where(s => s.StepNo < CurStepNo);
        foreach (var tag in prevTaskTags)
        {
            tag.Status = 2;
        }
        // 3. 设置后续步骤状态 (通常是未开始 Status=0，但也可能是历史里的已完成)
        var nextTaskTags = StationTaskLeftTagDataList.Where(s => s.StepNo > CurStepNo);
        foreach (var tag in nextTaskTags)
        {
            var task = StationTaskDataList.FirstOrDefault(t => t.StepNo == tag.StepNo);
            tag.Status = task.HasFinish ? 2 : 0;
        }
    }

    public async void DisableBoltGuns()
    {
        //foreach (var screwCtrl in _DesoutterMgr.DesoutterCtrls)
        //{
        //    await screwCtrl.Value.Disable();
        //}
    }

    /// <summary>
    /// 初始化页面数据
    /// </summary>
    private void InitPageV()
    {
        App._StepStationSetting.Step = null;
        App._StepStationSetting.Station = null;
        App.IsDealingRunAGV = false;
        App.UpCatlMesData = null;
        App.PackBarCode = string.Empty;
        App.PackOutCode = string.Empty;
        App.CurTaskName = string.Empty;
        App.ActivePage = null;
        App._ActivityStationTaskCommonPage = null;
        StationTaskLeftTagDataList = new ObservableCollection<StationTaskLeftTagData>();
        StationTaskDataList = new ObservableCollection<StationTaskData>();
        App.HisTaskData = null;
        App.HisTaskData2 = null;
        App.CurrentStationCode = string.Empty;
        this.StopTimer();
        DisableBoltGuns();

        //清空PLC请求
        _stationPLCContext.OverrunAlarm = false;
        _stationPLCContext.LetGo = false;
        _stationPLCContext.Initial = true;
        _stationPLCContext.StationTightenStartReqs.Clear();
    }

    #region 日志消息，报警日志消息

    public ObservableCollection<LogMessage> Logs { get; } = new();

    public ObservableCollection<AlarmMessage> AlarmLogs { get; } = new();

    public ObservableCollection<Alarm> Alarms { get; } = new();

    public ICommand AddLogMsg { get; }

    public ICommand UpdateAlarms { get; }

    public ICommand DeleteAlarm { get; }

    private bool _isLogsChanged;

    public bool IsLogsChanged
    {
        get => _isLogsChanged;
        set
        {
            this._isLogsChanged = value;
            this.OnPropertyChanged(nameof(IsLogsChanged));
        }
    }

    private bool _isAlarmsChanged;

    public bool IsAlarmsChanged
    {
        get => _isAlarmsChanged;
        set
        {
            this._isAlarmsChanged = value;
            this.OnPropertyChanged(nameof(IsAlarmsChanged));
        }
    }

    #endregion

    /// <summary>
    /// 任务数据源
    /// </summary>
    public ObservableCollection<StationTaskData> StationTaskDataList
    {
        get => _StationTaskDataList;
        set
        {
            if (_StationTaskDataList != value)
            {
                _StationTaskDataList = value;
                OnPropertyChanged(nameof(StationTaskDataList));
            }
        }
    }

    private ObservableCollection<StationTaskData> _StationTaskDataList;

    private int _CurStepNo;

    public int CurStepNo
    {
        get => _CurStepNo;
        set
        {
            if (_CurStepNo != value)
            {
                _CurStepNo = value;
                SelectedIndex = _CurStepNo - 1;
                OnPropertyChanged(nameof(CurStepNo));
            }
        }
    }

    private string _CurStationTaskHeader;

    public string CurStationTaskHeader
    {
        get => _CurStationTaskHeader;
        set
        {
            if (_CurStationTaskHeader != value)
            {
                _CurStationTaskHeader = value;
                OnPropertyChanged(nameof(CurStationTaskHeader));
            }
        }
    }


    private int _SelectedIndex;

    public int SelectedIndex
    {
        get => _SelectedIndex;
        set
        {
            if (_SelectedIndex != value)
            {
                _SelectedIndex = value;
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }
    }

    public bool HasReWork { get; internal set; }

    private int _CurrentPackUsedTime;

    public int CurrentPackUsedTime
    {
        get => _CurrentPackUsedTime;
        set
        {
            _CurrentPackUsedTime = value;
            OnPropertyChanged(nameof(CurrentPackUsedTime));
        }
    }

    public string LastPackCode { get; set; } = string.Empty;
    public ICommand Bingagv { get; }
    public ICommand InitPage { get; }
    public ICommand SkipStep { get; }
    public ICommand UploadAgain { get; }
}
