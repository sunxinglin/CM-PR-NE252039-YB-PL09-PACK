using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Threading;

using AsZero.Core.Services.Messages;

using Ctp0600P.Client.Apis;
using Ctp0600P.Client.Protocols.BoltGun.Models;
using Ctp0600P.Client.Protocols.ScanCode.Models;
using Ctp0600P.Client.ViewModels;
using Ctp0600P.Client.Views.StationTaskPages;
using Ctp0600P.Client.Views.Windows;

using MediatR;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Yee.Common.Library.CommonEnum;
using Yee.Services.CatlMesInvoker;

namespace Ctp0600P.Client.Views.Pages;

public partial class RealtimePage_OutLine : RealTimeCommonPage
{
    private IMediator _mediator;
    public DispatcherTimer WorkingTime;

    public RealtimePage_OutLine(RealtimePageViewModel vm, APIHelper apiHelper, ICatlMesInvoker catlMesInvoker, IMediator mediator) : base(mediator)
    {
        InitializeComponent();
        Vm = vm;
        this.DataContext = vm;
        ApiHelper = apiHelper;
        CatlMesInvoker = catlMesInvoker;
        _mediator = mediator;
        this.Vm.LetScrollBarGo = LetScrollBarGo;
        this.WorkingTime = new DispatcherTimer();
        WorkingTime.Interval = TimeSpan.FromSeconds(1);

        WorkingTime.Tick += (s, e) =>
        {
            this.Vm.CurrentPackUsedTime++;
        };

        this.Vm.TimerWorking.Select(s => s).ToEvent().OnNext += (s) =>
        {
            if (s)
            {
                this.WorkingTime.Start();
            }
            else
            {
                this.WorkingTime.Stop();
            }
        };
    }

    /// <summary>
    /// 执行下一个任务
    /// </summary>
    /// <param name="isNext"></param>
    public void GotoNextTask(bool isNext)
    {

        if (isNext)
            Vm.CurStepNo += 1;
        else
            Vm.CurStepNo -= 1;

        Vm.DealTaskStep();
    }

    private void Button_Click_ReWork(object sender, RoutedEventArgs e)
    {
        Vm.ReWork();
    }

    private async void ScanCodeBtnTest(object sender, RoutedEventArgs e)
    {
        var inputDialog = new InputWindow();
        var ok = inputDialog.ShowDialog() ?? false;
        if (!ok)
        {
            return;
        }

        var request = new ScanCodeGunRequest
        {
            ScanCodeContext = inputDialog.GetInputValue(),
            ScanCodePortName = "5000"
        };
        await _mediator.Publish(request);
        App.ScanCodeGunRequestSubject.OnNext(request);
    }

    private void ScrewBtnTest(object sender, RoutedEventArgs e)
    {
        var data = new BoltGunRequest
        {
            AngleStatus = 1,
            Angle_Max = 150,
            Angle_Min = 20,
            DeviceNo = "1",
            FinalAngle = 90,
            FinalTorque = 10,
            ProgramNo = 10,
            ResultIsOK = true,
            TargetAngle = 150,
            TargetTorqueRate = 10,
            TorqueRate_Max = 15,
            TorqueRate_Min = 5,
            TorqueStatus = 1,
        };
        var msg = new LogMessage
        {
            Content = $"当前拧紧结果结果：{JsonConvert.SerializeObject(data)}",
            Level = LogLevel.Information,
            Timestamp = DateTime.Now,
        };
        _mediator.Publish(new UILogNotification(msg));
        App.BoltGunRequestSubject.OnNext(data);
    }

    private void LetGoBtnTest(object sender, RoutedEventArgs e)
    {
        ((LetGoPage)App.ActivePage).CatchIOMessage(IOEnum.放行);
    }


    private void ResetBtnTest(object sender, RoutedEventArgs e)
    {
        App.StationTaskNGPause = false;
        App._RealtimePage.Vm.Alarms.Clear();
    }

    private void LetScrollBarGo(int stepNo)
    {
        this.leftScroll.ScrollToVerticalOffset(stepNo * 35);
    }

    private void unbingagv_Click(object sender, RoutedEventArgs e)
    {
        Vm.UnBingAGVPack();
    }

    private void bingagv_Click(object sender, RoutedEventArgs e)
    {
        //_VM.BingAGVPAck();
    }
}
