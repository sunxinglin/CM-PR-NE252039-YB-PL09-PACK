using Automatic.Client.ViewModels.Realtime;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;

namespace Automatic.Client.Views.Realtime
{
    /// <summary>
    /// TerminalReshapeMonitorView.xaml 的交互逻辑
    /// </summary>
    public partial class TerminalReshapeMonitorView : UserControl, IViewFor<TerminalReshapeMonitorViewModel>
    {
        public TerminalReshapeMonitorView()
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
                this.OneWayBind(this.ViewModel, vm => vm.EnterVectorCode, v => v.enterVectorCode.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.EnterPackCode, v => v.enterPackCode.Text).DisposeWith(d);

                this.OneWayBind(this.ViewModel, vm => vm.ReqStart, v => v.startReq.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartVectorCode, v => v.startAGVNo.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartPackCode, v => v.startPackCode.Text).DisposeWith(d);

                this.OneWayBind(this.ViewModel, vm => vm.ReqComplete, v => v.completeReq.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ComplateVectorCode, v => v.completeAGVNo.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ComplatePackCode, v => v.completePackCode.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ReshapeStartTime, v => v.glueStartTime.Text).DisposeWith(d);

                this.OneWayBind(this.ViewModel, vm => vm.VectorEnterAck, v => v.agvEnterAck.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.VectorEnterOK, v => v.agvEnterOK.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.EnterReshapeLevel, v => v.enterReshapeLevel.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.VectorEnterNG, v => v.agvEnterNG.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.EnterErrorCode, v => v.enterErrorCode.Text).DisposeWith(d);

                this.OneWayBind(this.ViewModel, vm => vm.StartReshapeAck, v => v.startReqAck.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartReshapeOK, v => v.startReqOK.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartReshapeLevel, v => v.startReshapeLevel.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartReshapeNG, v => v.startReqNG.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartErrorCode, v => v.startErrorCode.Text).DisposeWith(d);

                this.OneWayBind(this.ViewModel, vm => vm.CompleteReshapeAck, v => v.completeAck.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.CompleteReshapeOK, v => v.completeOK.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.CompleteReshapeNG, v => v.completeNG.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ComplateErrorCode, v => v.complateErrorCode.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.PressureValues, v => v.ReshapeData.ItemsSource).DisposeWith(d);
            });
        }



        public TerminalReshapeMonitorViewModel ViewModel
        {
            get { return (TerminalReshapeMonitorViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel { get => this.ViewModel; set => this.ViewModel = (TerminalReshapeMonitorViewModel)value!; }


        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(TerminalReshapeMonitorViewModel), typeof(TerminalReshapeMonitorView), new PropertyMetadata(null));



    }
}
