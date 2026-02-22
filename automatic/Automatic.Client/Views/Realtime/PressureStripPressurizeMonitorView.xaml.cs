using Automatic.Client.ViewModels.Realtime;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;

namespace Automatic.Client.Views.Realtime
{
    /// <summary>
    /// PressureStripMonitorView.xaml 的交互逻辑
    /// </summary>
    public partial class PressureStripPressurizeMonitorView : UserControl, IViewFor<PressureStripPressurizeMonitorViewModel>
    {
        public PressureStripPressurizeMonitorView()
        {
            InitializeComponent();

            this.DataContext = this;
            this.WhenActivated(d =>
            {
                //通用
                this.OneWayBind(this.ViewModel, vm => vm.MstHeartBeatReq, v => v.MESHeartBeatReq.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.DevHeartBeatAck, v => v.PLCHeartBeatAck.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.DevHeartBeatReq, v => v.PLCHeartBeatReq.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.MstHeartBeatAck, v => v.MESHeartBeatAck.IsChecked).DisposeWith(d);

                this.OneWayBind(this.ViewModel, vm => vm.DevAuto, v => v.AutoMode.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.DevManual, v => v.ManualMode.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.DevStop, v => v.StopMode.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.DevBusy, v => v.Busy.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.DevIdle, v => v.Idle.IsChecked).DisposeWith(d);

                //业务
                this.OneWayBind(this.ViewModel, vm => vm.ReqVectorEnter, v => v.agvEnterReq.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.EnterVectorCode, v => v.agvEnterNo.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.EnterPackCode, v => v.agvEnterPackCode.Text).DisposeWith(d);

                this.OneWayBind(this.ViewModel, vm => vm.ReqStart, v => v.startReq.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartVectorCode, v => v.startAGVNo.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartPackCode, v => v.startPackCode.Text).DisposeWith(d);

                this.OneWayBind(this.ViewModel, vm => vm.ReqComplete, v => v.completeReq.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ComplateVectorCode, v => v.completeAGVNo.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ComplatePackCode, v => v.completePackCode.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.CompleteTime, v => v.completePressureTime.Text).DisposeWith(d);

               


                this.OneWayBind(this.ViewModel, vm => vm.VectorEnterAck, v => v.agvEnterAck.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.VectorEnterOK, v => v.agvEnterOK.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.EnterPressureLevel, v => v.enterPressureLevel.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.VectorEnterNG, v => v.agvEnterNG.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.EnterErrorCode, v => v.enterErrorCode.Text).DisposeWith(d);

                this.OneWayBind(this.ViewModel, vm => vm.StartAck, v => v.startReqAck.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartOK, v => v.startReqOK.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartPressureLevel, v => v.startPressureLevel.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartNG, v => v.startReqNG.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartErrorCode, v => v.startErrorCode.Text).DisposeWith(d);

                this.OneWayBind(this.ViewModel, vm => vm.CompleteAck, v => v.completeAck.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.CompleteOK, v => v.completeOK.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.CompleteNG, v => v.completeNG.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ComplateErrorCode, v => v.completeErrorCode.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.PressureValues, v => v.PressureData.ItemsSource).DisposeWith(d);
            });

        }


        #region ViewModel
        public PressureStripPressurizeMonitorViewModel ViewModel
        {
            get { return (PressureStripPressurizeMonitorViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel { get => this.ViewModel; set => this.ViewModel = (PressureStripPressurizeMonitorViewModel)value; }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(PressureStripPressurizeMonitorViewModel), typeof(PressureStripPressurizeMonitorView), new PropertyMetadata(null));
        #endregion
    }
}
