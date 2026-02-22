using Automatic.Client.ViewModels.Realtime;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;

namespace Automatic.Client.Views.Realtime
{
    /// <summary>
    /// ShoulderGlueMonitorView.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleInBoxMonitorView : UserControl, IViewFor<ModuleInBoxMonitorViewModelBase>
    {
        public ModuleInBoxMonitorView()
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
                //拍照完成
                this.OneWayBind(this.ViewModel, vm => vm.ReqTakePhotoComplete, v => v.reqTakePhotoComplete.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.TakePhotoCompleteCellCode, v => v.takePhotoCompleteCellCode.Text).DisposeWith(d);


                this.OneWayBind(this.ViewModel, vm => vm.AckTakePhotoComplete, v => v.ackTakePhotoComplete.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.TakePhotoCompleteOK, v => v.takePhotoCompleteOK.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ModuleCode, v => v.moduleCode.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.TakePhotoCompleteOK, v => v.takePhotoCompleteOK.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.TakePhotoCompleteNG, v => v.takePhotoCompleteNG.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.PhotoNgCode, v => v.photoNgCode.Text).DisposeWith(d);

                //小车进
                this.OneWayBind(this.ViewModel, vm => vm.ReqVectorEnter, v => v.reqVectorEnter.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.AckVectorEnter, v => v.ackVectorEnter.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.VectorEnterOk, v => v.vectorEnterOK.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.VectorEnterNg, v => v.vectorEnterNG.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.VectorEnterVectorCode, v => v.vectorEnterNo.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.VectorEnterPackCode, v => v.vectorEnterPackCode.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.GlueRemainDuration, v => v.vectorEnterGlueRemainDuration.Text).DisposeWith(d);

                this.OneWayBind(this.ViewModel, vm => vm.VectorNgCode, v => v.vectorNgCode.Text).DisposeWith(d);

                //开始入
                this.OneWayBind(this.ViewModel, vm => vm.ReqStartEnterInBox, v => v.reqStart.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.AckStartEnterInBox, v => v.ackStart.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartEnterInBoxOK, v => v.startOK.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartEnterInBoxNG, v => v.startNG.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartInBoxVectorCode, v => v.startAGVNo.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartInBoxPackCode, v => v.startPackCode.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.StartNgCode, v => v.startNgCode.Text).DisposeWith(d);

                //单个入完成
                this.OneWayBind(this.ViewModel, vm => vm.ReqSingleInBoxComplete, v => v.reqSingleInBoxComplete.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.SingleInBoxCompleteVectorCode, v => v.SingleInBoxCompleteAGVNo.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.SingleInBoxCompletePackCode, v => v.SingleInBoxCompletePackCode.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.SingleInBoxCompleteModuleCode, v => v.SingleInBoxCompleteModuleCode.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.SingleInBoxCompleteModuleLocation, v => v.SingleInBoxCompleteModuleLocation.Text).DisposeWith(d);

                this.OneWayBind(this.ViewModel, vm => vm.AckSingleInBoxComplete, v => v.ackSingleInBoxComplete.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.SingleInBoxCompleteOK, v => v.SingleInBoxCompleteOK.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.SingleInBoxCompleteNG, v => v.SingleInBoxCompleteNG.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.SingleInBoxCompleteErrorCode, v => v.SingleInBoxCompleteNgCode.Text).DisposeWith(d);

                //全部入完成
                this.OneWayBind(this.ViewModel, vm => vm.ReqStartEnterInBoxComplete, v => v.reqComplete.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.AckEnterInBoxComplete, v => v.ackComplete.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.EnterInBoxCompleteOK, v => v.completeOK.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.EnterInBoxCompleteNG, v => v.completeNG.IsChecked).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ComplateInBoxVectorCode, v => v.completeAGVNo.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ComplateInBoxPackCode, v => v.completePackCode.Text).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ModuleDatas, v => v.ModuleDatas.ItemsSource).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.EnterInBoxCompleteErrorCode, v => v.complateNgCode.Text).DisposeWith(d);

            });
        }


        #region ViewModel
        public ModuleInBoxMonitorViewModelBase ViewModel
        {
            get { return (ModuleInBoxMonitorViewModelBase)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel { get => this.ViewModel; set => this.ViewModel = (ModuleInBoxMonitorViewModelBase)value; }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(ModuleInBoxMonitorViewModelBase), typeof(ModuleInBoxMonitorView), new PropertyMetadata(null));
        #endregion

    }
}
