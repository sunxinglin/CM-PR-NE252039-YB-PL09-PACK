using Automatic.Client.ViewModels.Realtime;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;

namespace Automatic.Client.Views.Realtime
{
    /// <summary>
    /// LowerBoxGlueMonitorView.xaml 的交互逻辑
    /// </summary>
    public partial class LowerBoxGlueMonitorView : UserControl, IViewFor<LowerBoxGlueMonitorViewModel>
    {
        public LowerBoxGlueMonitorView()
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

                //进工位
                this.OneWayBind(this.ViewModel, vm => vm.ReqVectorEnter, v => v.agvEnterReq.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.EnterVectorCode, v => v.enterVectorCode.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.EnterPackCode, v => v.enterPackCode.Text).DisposeWith(d);

                this.OneWayBind(this.ViewModel, vm => vm.VectorEnterAck, v => v.agvEnterAck.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.VectorEnterOK, v => v.agvEnterOK.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.VectorEnterNG, v => v.agvEnterNG.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.EnterErrorCode, v => v.enterErrorCode.Text).DisposeWith(d);


                //开始涂胶
                this.OneWayBind(this.ViewModel, vm => vm.ReqStart, v => v.startReq.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartVectorCode, v => v.startAGVNo.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartPackCode, v => v.startPackCode.Text).DisposeWith(d);

                this.OneWayBind(this.ViewModel, vm => vm.StartGlueAck, v => v.startReqAck.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartGlueOK, v => v.startReqOK.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartGlueNG, v => v.startReqNG.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartErrorCode, v => v.startErrorCode.Text).DisposeWith(d);


                //涂胶完成
                this.OneWayBind(this.ViewModel, vm => vm.ReqComplete, v => v.completeReq.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ComplateVectorCode, v => v.completeAGVNo.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ComplatePackCode, v => v.completePackCode.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.GlueStartTime, v => v.glueStartTime.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.GlueDatas, v => v.GlueData.ItemsSource).DisposeWith(d);

                this.OneWayBind(this.ViewModel, vm => vm.CompleteGlueAck, v => v.completeAck.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.CompleteGlueOK, v => v.completeOK.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.CompleteGlueNG, v => v.completeNG.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ComplateErrorCode, v => v.complateErrorCode.Text).DisposeWith(d);


                //补胶完成
                this.OneWayBind(this.ViewModel, vm => vm.ReqReGlueComplete, v => v.reqReGlueComplete.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ReGlueCompleteVectorCode, v => v.reGlueCompleteVectorCode.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ReGlueCompletePackCode, v => v.reGlueCompletePackCode.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ReGlueCompleteStartTime, v => v.reGlueCompleteStartTime.Text).DisposeWith(d);

                this.OneWayBind(this.ViewModel, vm => vm.ReGlueCompleteAck, v => v.reGlueCompleteAck.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ReGlueCompleteOK, v => v.reGlueCompleteOK.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ReGlueCompleteNG, v => v.reGlueCompleteNG.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ReGlueCompleteErrorCode, v => v.reGluecomplateErrorCode.Text).DisposeWith(d);


                //开始补胶
                this.OneWayBind(this.ViewModel, vm => vm.ReqReGlueStart, v => v.reqReGlueStart.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ReGlueStartVectorCode, v => v.reGlueStartVectorCode.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ReGlueStartPackCode, v => v.reGlueStartPackCode.Text).DisposeWith(d);

                this.OneWayBind(this.ViewModel, vm => vm.ReGlueStartAck, v => v.reGlueStartAck.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ReGlueStartOK, v => v.reGlueStartOK.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ReGlueStartNG, v => v.reGlueStartNG.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ReGlueStartErrorCode, v => v.reGlueStartErrorCode.Text).DisposeWith(d);


                //this.OneWayBind(this.ViewModel, vm => vm.ReqInspect, v => v.inspectCompleteReq.IsChecked).DisposeWith(d);

                //this.OneWayBind(this.ViewModel, vm => vm.InspectAck, v => v.inspectCompleteAck.IsChecked).DisposeWith(d);
                //this.OneWayBind(this.ViewModel, vm => vm.InspectOk, v => v.inspectCompleteOK.IsChecked).DisposeWith(d);
                //this.OneWayBind(this.ViewModel, vm => vm.InspectNG, v => v.inspectCompleteNG.IsChecked).DisposeWith(d);
                //this.OneWayBind(this.ViewModel, vm => vm.InspectErrorCode, v => v.inspectCompleteErrorCode.Text).DisposeWith(d);
            });
        }


        #region ViewModel
        public LowerBoxGlueMonitorViewModel ViewModel
        {
            get { return (LowerBoxGlueMonitorViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel { get => this.ViewModel; set => this.ViewModel = (LowerBoxGlueMonitorViewModel)value; }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(LowerBoxGlueMonitorViewModel), typeof(LowerBoxGlueMonitorView), new PropertyMetadata(null));
        #endregion
        
    }
}
