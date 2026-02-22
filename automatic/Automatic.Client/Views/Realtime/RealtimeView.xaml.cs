using Automatic.Client.ViewModels.Realtime;
using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;

namespace Automatic.Client.Views.Realtime
{
    /// <summary>
    /// RealtimeView.xaml 的交互逻辑
    /// </summary>
    public partial class RealtimeView : UserControl, IViewFor<RealtimeViewModel>
    {
        public RealtimeView()
        {
            InitializeComponent();

            this.ViewModel = Locator.Current.GetService<RealtimeViewModel>();
            this.WhenActivated(d =>
            {
                this.OneWayBind(this.ViewModel, vm => vm.UIlogsViewModel, v => v.uilogView.ViewModel).DisposeWith(d);

                this.OneWayBind(this.ViewModel, vm => vm.HeatingFilmPressurizeMonitorViewModel, v => v.heatingFilmPressurizeMonitorView.ViewModel).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.LowerBoxGlueMonitorViewModel, v => v.lowerBoxGlueMonitorView.ViewModel).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ModuleInBoxMonitorViewModel, v => v.moduleInBoxMonitorView.ViewModel).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.ShoulderGlueMonitorViewModel, v => v.shoulderGlueMonitorView.ViewModel).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.PressureStripMonitorViewModel, v => v.pressureStripPressurizeMonitorView.ViewModel).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.UpperCoverTighten1MonitorViewModel, v => v.upperCoverTightenMonitorView.ViewModel).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.UpperCoverTighten2MonitorViewModel, v => v.upperCoverTighten2MonitorView.ViewModel).DisposeWith(d);
                //this.OneWayBind(this.ViewModel, vm => vm.ModuleTightenMonitorViewModel, v => v.moduleTightenMonitorView.ViewModel).DisposeWith(d);
                //this.OneWayBind(this.ViewModel, vm => vm.TerminalReshapeMonitorViewModel, v => v.terminalReshapeMonitorView.ViewModel).DisposeWith(d);
            });

            this.heatingFilmPressurize.Visibility = SetVisiblity(ViewModel?._scanOptsMonitor.Get("HeatingFilmPressurize")?.Enabled);
            this.lowerBoxGlue.Visibility = SetVisiblity(ViewModel?._scanOptsMonitor.Get("LowerBoxGlue")?.Enabled);
            this.moduleInBox.Visibility = SetVisiblity(ViewModel?._scanOptsMonitor.Get("ModuleInBox")?.Enabled);
            this.moduleInBox2.Visibility = SetVisiblity(ViewModel?._scanOptsMonitor.Get("ModuleInBox2")?.Enabled);
            this.moduleInBox3.Visibility = SetVisiblity(ViewModel?._scanOptsMonitor.Get("ModuleInBox3")?.Enabled);
            this.shoudlerGlue.Visibility = SetVisiblity(ViewModel?._scanOptsMonitor.Get("ShoulderGlue")?.Enabled);
            this.pressureStripPressurize.Visibility = SetVisiblity(ViewModel?._scanOptsMonitor.Get("PressureStripPressurize")?.Enabled);
            this.upperCoverTighten.Visibility = SetVisiblity(ViewModel?._scanOptsMonitor.Get("UpperCoverTighten")?.Enabled);
            this.upperCoverTighten2.Visibility = SetVisiblity(ViewModel?._scanOptsMonitor.Get("UpperCoverTighten2")?.Enabled);
            //this.terminalReshape.Visibility = SetVisiblity(ViewModel?._scanOptsMonitor.Get("TerminalReshape")?.Enabled);
            //this.moduleTighten.Visibility = SetVisiblity(ViewModel?._scanOptsMonitor.Get("ModuleTighten")?.Enabled);
            this.pageSelected.SelectedItem = GetVisiableItem();
        }


        #region ViewModel
        public RealtimeViewModel ViewModel
        {
            get { return (RealtimeViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel { get => this.ViewModel; set => this.ViewModel = (RealtimeViewModel)value; }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(RealtimeViewModel), typeof(RealtimeView), new PropertyMetadata(null));
        #endregion

        public Visibility SetVisiblity(bool? Enable)
        {
            if (Enable == null) return Visibility.Collapsed;
            return (bool)Enable ? Visibility.Visible : Visibility.Collapsed;
        }

        public TabItem GetVisiableItem()
        {
            if (this.heatingFilmPressurize.Visibility == Visibility.Visible) return this.heatingFilmPressurize;
            if (this.lowerBoxGlue.Visibility == Visibility.Visible) return this.lowerBoxGlue;
            if (this.moduleInBox.Visibility == Visibility.Visible) return this.moduleInBox;
            if (this.shoudlerGlue.Visibility == Visibility.Visible) return this.shoudlerGlue;
            if (this.pressureStripPressurize.Visibility == Visibility.Visible) return this.pressureStripPressurize;
            if (this.upperCoverTighten.Visibility == Visibility.Visible) return this.upperCoverTighten;
            if (this.upperCoverTighten2.Visibility == Visibility.Visible) return this.upperCoverTighten2;
            //if (this.moduleTighten.Visibility == Visibility.Visible) return this.moduleTighten;
            //if (this.terminalReshape.Visibility == Visibility.Visible) return this.terminalReshape;

            return this.lowerBoxGlue;
        }
    }
}
