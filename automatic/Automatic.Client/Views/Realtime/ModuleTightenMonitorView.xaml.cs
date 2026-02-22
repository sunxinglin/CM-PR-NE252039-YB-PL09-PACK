using Automatic.Client.ViewModels.Realtime;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;

namespace Automatic.Client.Views.Realtime
{
    /// <summary>
    /// AutoModuleTightenMonitorView.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleTightenMonitorView : UserControl, IViewFor<ModuleTightenMonitorViewModel>
    {
        public ModuleTightenMonitorView()
        {
            InitializeComponent();
            this.DataContext = this;
            this.WhenActivated(d =>
            {
                #region 通用
                this.OneWayBind(this.ViewModel, vm => vm.MstHeartBeatReq, v => v.MESHeartBeatReq.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.DevHeartBeatAck, v => v.PLCHeartBeatAck.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.DevHeartBeatReq, v => v.PLCHeartBeatReq.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.MstHeartBeatAck, v => v.MESHeartBeatAck.IsChecked).DisposeWith(d);

                this.OneWayBind(this.ViewModel, vm => vm.DevAuto, v => v.AutoMode.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.DevManual, v => v.ManualMode.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.DevStop, v => v.StopMode.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.DevBusy, v => v.Busy.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.DevIdle, v => v.Idle.IsChecked).DisposeWith(d);
                #endregion

                #region 业务
                this.OneWayBind(this.ViewModel, vm => vm.ReqVectorEnter, v => v.agvEnterReq.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ReqEnterVectorCode, v => v.agvEnterNo.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ReqEnterPackCode, v => v.agvEnterPackCode.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.AckVectorEnter, v => v.agvEnterAck.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.VectorEnterOK, v => v.agvEnterOK.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.VectorEnterNG, v => v.agvEnterNG.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.VectorEnterTightenLevel, v => v.enterLevel.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.VectorEnterErrorCode, v => v.enterErrorCode.Text).DisposeWith(d);


                this.OneWayBind(this.ViewModel, vm => vm.ReqStartTighten, v => v.startReq.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartTightenVectorCode, v => v.startAGVNo.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartTightenPackCode, v => v.startPackCode.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.AckStartTighten, v => v.startReqAck.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartTightenOK, v => v.startReqOK.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartTightenNG, v => v.startReqNG.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartTightenLevel, v => v.startLevel.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartTightenErrorCode, v => v.startErrorCode.Text).DisposeWith(d);

                this.OneWayBind(this.ViewModel, vm => vm.ReqComplateTighten, v => v.completeReq.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ComplateTightenVectorCode, v => v.completeAGVNo.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ComplateTightenPackCode, v => v.completePackCode.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.AckComplateTighten, v => v.completeAck.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ComplateTightenOK, v => v.completeOK.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ComplateTightenNG, v => v.completeNG.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ComplateTightenErrorCode, v => v.completeErrorCode.Text).DisposeWith(d);

                this.OneWayBind(this.ViewModel, vm => vm.tighteningData, v => v.TightenData.ItemsSource).DisposeWith(d);
                #endregion


            });
        }

        public ModuleTightenMonitorViewModel? ViewModel
        {
            get => (ModuleTightenMonitorViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
        object IViewFor.ViewModel { get => this.ViewModel!; set => this.ViewModel = (ModuleTightenMonitorViewModel)value; }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ModuleTightenMonitorViewModel), typeof(ModuleTightenMonitorView), new PropertyMetadata(null));


    }
}
