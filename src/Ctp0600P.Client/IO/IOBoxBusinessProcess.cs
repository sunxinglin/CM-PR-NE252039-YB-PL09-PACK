using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using Ctp0600P.Client.PLC.Context;
using Ctp0600P.Client.PLC.PLC01.Models.Notifications;
using Ctp0600P.Client.ViewModels;
using Ctp0600P.Client.Views.StationTaskPages;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using Yee.Common.Library.CommonEnum;

namespace Ctp0600P.Client.IO;

public class IOBoxBusinessProcess
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMediator _mediator;
    private readonly StationPLCContext _stationPLCContext;

    public IOBoxBusinessProcess(IServiceProvider service, IMediator mediator, StationPLCContext stationPLCContext)
    {
        _serviceProvider = service;
        _mediator = mediator;
        _stationPLCContext = stationPLCContext;
    }

    public bool hasDeal { get; set; } = true;
    private bool 自动 = false;
    private bool 手动 = false;
    private bool 风扇 = false;
    private int Tag_AGVRun = 0;
    private int StartDown = 0;
    private int ResetDown = 0;
    private const int Interval = 500;

    public async Task Process(StationPLCStatusNotification notification)
    {
        if (!hasDeal && App._RealtimePage != null && App._RealtimePage.Vm.Alarms.Count == 0)
        {
            //await ioBoxApi.OnRedLed(DoStatusEnum.OFF);
            //await ioBoxApi.OnBuzzer(DoStatusEnum.OFF);
            hasDeal = true;
        }

        if (hasDeal)
        {
            #region 手自动
            //自动
            if (notification.AutoMode && !notification.ManualMode && !自动)
            {
                自动 = true;
                手动 = false;
                /*Application.Current.Dispatcher.Invoke(() =>
                {
                    //关闭拧紧枪程序号下发界面
                    using var scope = _serviceProvider.CreateScope();
                    var sp = scope.ServiceProvider;
                    var manualblot = sp.GetRequiredService<ManualBolt>();
                    try
                    {
                        if (manualblot.Visibility == Visibility.Visible)
                        {
                            manualblot._vm.DisableBoltGuns();
                            manualblot.Close();
                            if (App.ActivePage != null && App.ActivePage.GetType() == typeof(BoltGun))
                            {
                                ((BoltGun)App.ActivePage)._VM.EnableBoltGuns();

                                _mediator.Publish(new UILogNotification(new LogMessage { Content = $"[IOBoxBusinessProcess]调用了[EnableBoltGuns] for [AutoMode]", Level = LogLevel.Trace, Timestamp = DateTime.Now }));

                            }
                        }
                    }
                    catch
                    {
                    }
                });
                await ioBoxApi.OnYellowLed(DoStatusEnum.OFF);
                await ioBoxApi.OnGreenLed(DoStatusEnum.ON);
                await ioBoxApi.OnRedLed(DoStatusEnum.OFF);
                await ioBoxApi.OnBuzzer(DoStatusEnum.OFF);*/
            }
            //手动
            if (!notification.AutoMode && notification.ManualMode && !手动)
            {
                手动 = true;
                自动 = false;
                /*if (App.ActivePage != null && (App.ActivePage.GetType() == typeof(BoltGun) || App.ActivePage.GetType() == typeof(RepairBoltGunCommon)))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        using var scope = _serviceProvider.CreateScope();
                        var sp = scope.ServiceProvider;
                        var wincheck = sp.GetRequiredService<CheckPowerPage>();
                        ((CheckPowerViewModel)wincheck.DataContext).Action = delegate
                        {
                            var sp = scope.ServiceProvider;
                            var manualblot = sp.GetRequiredService<ManualBolt>();
                            manualblot.Show();
                        };
                        ((CheckPowerViewModel)wincheck.DataContext).ModuleName = "ManualMode";
                        wincheck.Show();
                    });
                }

                await ioBoxApi.OnGreenLed(DoStatusEnum.OFF);
                await ioBoxApi.OnYellowLed(DoStatusEnum.ON);
                await ioBoxApi.OnRedLed(DoStatusEnum.OFF);
                await ioBoxApi.OnBuzzer(DoStatusEnum.OFF);*/
            }

            #endregion

            //放行
            if (notification.LetGo && StartDown <= 0)
            {
                Interlocked.Exchange(ref StartDown, Interval);
                //await ioBoxApi.StartAction(DoStatusEnum.ON);
                if (App.ActivePage != null && App.ActivePage.GetType() == typeof(LetGoPage) && Interlocked.Exchange(ref Tag_AGVRun, 1) == 0)
                {
                    ((LetGoPage)App.ActivePage).CatchIOMessage(IOEnum.放行);
                    Interlocked.Exchange(ref Tag_AGVRun, 0);
                }
                else
                {
                    await _mediator.Publish(new MessageNotice { showType = MessageShowType.右下角弹窗, messageType = MessageTypeEnum.警告, MessageStr = "当前尚未处于AGV放行任务下！或正在放行AGV" });
                }
            }
            else if (!notification.LetGo && StartDown > 0)
            {
                await Task.Delay(100);//防抖
                if (!notification.LetGo)
                {
                    //await ioBoxApi.StartAction(DoStatusEnum.OFF);
                    Interlocked.Exchange(ref StartDown, 0);
                }

            }
        }
        if (notification.Reset && ResetDown <= 0)
        {
            //await ioBoxApi.ResetAction(DoStatusEnum.ON);
            Interlocked.Exchange(ref ResetDown, Interval);
            MakesReset(true);
        }
        else if (!notification.Reset)
        {
            await Task.Delay(100);//防抖
            if (!notification.Reset)
            {
                //await ioBoxApi.ResetAction(DoStatusEnum.OFF);
                Interlocked.Exchange(ref ResetDown, 0);
            }
        }

        #region 错误

        if (风扇 && !notification.Fan)
        {
            hasDeal = false;
        }

        #endregion

        if (!hasDeal)
        {
            MakesAlarm();
        }
        // 清除报警
        _stationPLCContext.Alarm = !hasDeal;
    }

    /// <summary>
    /// 发生错误
    /// </summary>
    /// <returns></returns>
    public void MakesAlarm()
    {

        手动 = false;
        自动 = false;
        //await ioBoxApi.OnYellowLed(DoStatusEnum.OFF);
        //await ioBoxApi.OnGreenLed(DoStatusEnum.OFF);
        //await ioBoxApi.OnRedLed(DoStatusEnum.ON);
        //await ioBoxApi.OnBuzzer(DoStatusEnum.ON);
    }

    /// <summary>
    /// 复位
    /// </summary>
    /// <returns></returns>
    public void MakesReset(bool resetaction)
    {
        try
        {
            Interlocked.Increment(ref ResetDown);
            Application.Current.Dispatcher.Invoke(() =>
            {
                using var scope = _serviceProvider.CreateScope();
                var sp = scope.ServiceProvider;
                var _realtimeManualvm = sp.GetRequiredService<RealtimePageViewModel>();
                _realtimeManualvm.DealResetAlarm();
            });

            if (App._RealtimePage == null)
            {
                hasDeal = true;
                return;
            }

            if (App._RealtimePage.Vm.Alarms.Count == 0)
            {
                //await ioBoxApi.OnRedLed(DoStatusEnum.OFF);
                //await ioBoxApi.OnBuzzer(DoStatusEnum.OFF);
                hasDeal = true;
            }

            Interlocked.Exchange(ref ResetDown, 0);
        }
        catch
        {
            hasDeal = true;
        }
    }
}