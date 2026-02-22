using Ctp0600P.Client.Apis;
using Ctp0600P.Client.ViewModels;
using Ctp0600P.Client.Views.Windows;
using MediatR;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Threading;
using Yee.Services.CatlMesInvoker;

namespace Ctp0600P.Client.Views.Pages
{
    /// <summary>
    /// Interaction logic for RealtimePage_OutLine.xaml
    /// </summary>
    public partial class RealtimePage_OutLine : RealTimeCommonPage
    {
        private IMediator _mediator;
        public DispatcherTimer WorkingTime;

        public RealtimePage_OutLine(RealtimePageViewModel vm, APIHelper apiHepler, ICatlMesInvoker catlMesInvoker, IMediator mediator) : base(mediator)
        {
            InitializeComponent();
            _VM = vm;
            this.DataContext = vm;
            _APIHepler = apiHepler;
            _catlMesInvoker = catlMesInvoker;
            _mediator = mediator;
            this._VM.LetScrollBarGo = LetScrollBarGo;
            this.WorkingTime = new DispatcherTimer();
            WorkingTime.Interval = TimeSpan.FromSeconds(1);

            WorkingTime.Tick += (s, e) =>
            {
                this._VM.CurrentPackUsedTime++;
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

        private void Button_Click_Scan1(object sender, RoutedEventArgs e)
        {
            App.ScanCodeGunRequestSubject.OnNext(new Protocols.ScanCode.Models.ScanCodeGunRequest
            {
                ScanCodeContext = "001PE0X300000DE8H0180017",
                ScanCodePortName = "5000"
            });
        }

        private void Button_Click_Scan2(object sender, RoutedEventArgs e)
        {
            App.BoltGunRequestSubject.OnNext(new Protocols.BoltGun.Models.BoltGunRequest
            {
                AngleStatus = 1,
                Angle_Max = 20,
                Angle_Min = 20,
                DeviceNo = "1",
                FinalAngle = 10,
                FinalTorque = 10,
                ProgramNo = 6,
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
            App.ScanCodeGunRequestSubject.OnNext(new Protocols.ScanCode.Models.ScanCodeGunRequest
            {
                ScanCodeContext = "123456789000",
                ScanCodePortName = "5000"
            });
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
        }

        private void LetScrollBarGo(int stepNo)
        {
            this.leftScroll.ScrollToVerticalOffset(stepNo * 35);
        }

        private void unbingagv_Click(object sender, RoutedEventArgs e)
        {
            _VM.UnBingAGVPack();
        }

        private void bingagv_Click(object sender, RoutedEventArgs e)
        {
            //_VM.BingAGVPAck();
        }
    }
}
