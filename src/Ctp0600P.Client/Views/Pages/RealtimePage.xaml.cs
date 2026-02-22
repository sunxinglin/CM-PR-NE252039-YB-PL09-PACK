using AsZero.Core.Services.Messages;
using Ctp0600P.Client.Apis;
using Ctp0600P.Client.PLC.Context;
using Ctp0600P.Client.ViewModels;
using Ctp0600P.Client.Views.StationTaskPages;
using Ctp0600P.Client.Views.Windows;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Threading;
using Yee.Common.Library.CommonEnum;
using Yee.Services.CatlMesInvoker;

namespace Ctp0600P.Client.Views.Pages
{
    /// <summary>
    /// Interaction logic for RealtimePage.xaml
    /// </summary>
    public partial class RealtimePage : RealTimeCommonPage
    {
        private IMediator _mediator;
        public DispatcherTimer WorkingTime;
        private readonly StationPLCContext _stationPLCContext;

        public RealtimePage(RealtimePageViewModel vm, APIHelper apiHepler, ICatlMesInvoker catlMesInvoker, IMediator mediator, StationPLCContext stationPLCContext) : base(mediator)
        {
            _VM = vm;
            this.DataContext = vm;
            InitializeComponent();
            _APIHepler = apiHepler;
            _catlMesInvoker = catlMesInvoker;
            _mediator = mediator;
            _stationPLCContext = stationPLCContext;

            this._VM.LetScrollBarGo = LetScrollBarGo;

            this.WorkingTime = new DispatcherTimer();
            WorkingTime.Interval = TimeSpan.FromSeconds(1);

            WorkingTime.Tick += (s, e) =>
            {
                this._VM.CurrentPackUsedTime++;
                _stationPLCContext.OverrunAlarm = App._StepStationSetting.OverrunAlarmStartSecond != 0 && _VM.CurrentPackUsedTime > App._StepStationSetting.OverrunAlarmStartSecond;
            };
            this._VM.TimerWorking.Select(s => s).ToEvent().OnNext += (s) =>
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
                _VM.CurStepNo += 1;
            else
                _VM.CurStepNo -= 1;

            _VM.DealTaskStep();
        }

        private void Button_Click_ReWork(object sender, RoutedEventArgs e)
        {
            _VM.ReWork();
        }

        private void LetScrollBarGo(int stepNo)
        {
            this.leftScroll.ScrollToVerticalOffset(stepNo * 35);
        }

        private async void Button_Click_Scan1(object sender, RoutedEventArgs e)
        {
            //App.ScanCodeGunRequestSubject.OnNext(new Protocols.ScanCode.Models.ScanCodeGunRequest
            //{
            //    //ScanCodeContext = "123456123456",
            //    ScanCodeContext = "123456AAAAAA",
            //    ScanCodePortName = "5000"
            //});
            //App.CatchScanCodeMessage(new Protocols.ScanCode.Models.ScanCodeGunRequest
            //{
            //    //ScanCodeContext = "123456123456123456",
            //    ScanCodeContext = "222999999999",
            //    ScanCodePortName = "5000"
            //});
            //App.CatchScanCodeMessage(new Protocols.ScanCode.Models.ScanCodeGunRequest
            //{
            //    //ScanCodeContext = "123456123456",
            //    ScanCodeContext = "001PE0X300000DE8K0180010",
            //    ScanCodePortName = "5000"
            //});
            //App.StationTaskNGPause = false;
            //App._RealtimePage._VM.Alarms.Clear();
            //((LetGoPage)App.ActivePage).CatchIOMessage(IOEnum.放行);
            this._VM.DealRunAGV();
        }

        private void Button_Click_Scan2(object sender, RoutedEventArgs e)
        {
            var data = new Protocols.BoltGun.Models.BoltGunRequest
            {
                AngleStatus = 1,
                Angle_Max = 20,
                Angle_Min = 20,
                DeviceNo = "1",
                FinalAngle = 10,
                FinalTorque = 10,
                ProgramNo = 10,
                ResultIsOK = true,
                TargetAngle = 10,
                TargetTorqueRate = 10,
                TorqueRate_Max = 10,
                TorqueRate_Min = 10,
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

        private void Button_Click_Screw3(object sender, RoutedEventArgs e)
        {
            App.BoltGunRequestSubject.OnNext(new Protocols.BoltGun.Models.BoltGunRequest
            {
                AngleStatus = 1,
                Angle_Max = 20,
                Angle_Min = 20,
                DeviceNo = "2",
                FinalAngle = 10,
                FinalTorque = 10,
                ProgramNo = 12,
                ResultIsOK = true,
                TargetAngle = 10,
                TargetTorqueRate = 10,
                TorqueRate_Max = 10,
                TorqueRate_Min = 10,
                TorqueStatus = 1,
            });
        }

        private void Button_Click_Scan3(object sender, RoutedEventArgs e)
        {
            ((LetGoPage)App.ActivePage).CatchIOMessage(IOEnum.放行);
        }

        private async void Button_Click_Scan4(object sender, RoutedEventArgs e)
        {
            var inputDialog = new InputWindow();
            var result = inputDialog.ShowDialog() ?? false;
            if (!result)
            {
                return;
            }
            var msg = new Protocols.ScanCode.Models.ScanCodeGunRequest
            {
                ScanCodeContext = $"{inputDialog.GetInputValue()}",
                ScanCodePortName = "5000"
            };
            await _mediator.Publish(msg);
            //App.ScanCodeGunRequestSubject.OnNext(msg);
        }

        private void unbingagv_Click(object sender, RoutedEventArgs e)
        {
            _VM.UnBingAGVPack();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            App.ScanCodeGunRequestSubject.OnNext(new Protocols.ScanCode.Models.ScanCodeGunRequest
            {
                ScanCodeContext = "001PE0X300000LE7G0111111",
                ScanCodePortName = "5000"
            });
        }
    }
}
