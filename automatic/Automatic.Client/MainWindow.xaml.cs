using Automatic.Client.ViewModels;
using ReactiveUI;
using Splat;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Automatic.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewFor<MainViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            this.ViewModel = Locator.Current.GetService<MainViewModel>();
            this.WhenActivated(d => {

                this.OneWayBind(this.ViewModel, vm => vm.AppViewModel.AppTitle, v => v.Title).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.AppViewModel.AppTitle, v => v.txtTitle.Text).DisposeWith(d);
                //this.OneWayBind(this.ViewModel, vm => vm.AppViewModel.EquipId, v => v.txtEquipId.Text).DisposeWith(d);
                //this.OneWayBind(this.ViewModel, vm => vm.AppViewModel.UserName, v => v.txtUserName.Text).DisposeWith(d);

                //this.OneWayBind(this.ViewModel, vm => vm.AppViewModel.CanAccessUserMgmt, v => v.menuUserMgmt.IsEnabled).DisposeWith(d);
                //this.OneWayBind(this.ViewModel, vm => vm.AppViewModel.CanAccessParamsSetting, v => v.menuSettings.IsEnabled).DisposeWith(d);
                //this.OneWayBind(this.ViewModel, vm => vm.AppViewModel.CanAccessParamsSetting, v => v.menuDbgTool.IsEnabled).DisposeWith(d);

                this.OneWayBind(this.ViewModel, vm => vm.AppViewModel.Router, x => x.routedHost.Router).DisposeWith(d);

                this.ViewModel.AppViewModel.Router.Navigate.ThrownExceptions
                    .Subscribe(e => MessageBox.Show(e.Message))
                    .DisposeWith(d);
            });
        }

        #region
        public MainViewModel ViewModel
        {
            get { return (MainViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(MainViewModel), typeof(MainWindow), new PropertyMetadata(null));

        object IViewFor.ViewModel { get => this.ViewModel; set => this.ViewModel = (MainViewModel)value; }
        #endregion

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var res = MessageBox.Show("是否真的要退出？", "退出程序确认", MessageBoxButton.YesNo);
            if (!res.Equals(MessageBoxResult.Yes))
            {
                e.Cancel = true;
            }
        }

        private void ToggleLeftNavBarShowHiden(object sender, MouseButtonEventArgs e)
        {
            //if (left.Visibility != Visibility.Visible)
            //{
            //    left.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    left.Visibility = Visibility.Collapsed;
            //}
            e.Handled = true;
        }

        private void MenuClick_Realtime(object sender, RoutedEventArgs e)
        {
            this.ViewModel.AppViewModel.NavigateTo(UrlDefines.URL_Realtime);
        }

        //private void MenuClick_UserMgmt(object sender, RoutedEventArgs e)
        //{
        //    this.ViewModel.AppViewModel.NavigateTo(UrlDefines.URL_UserMgmt);
        //}

        //private void MenuClick_ParamsMgmt(object sender, RoutedEventArgs e)
        //{
        //    this.ViewModel.AppViewModel.NavigateTo(UrlDefines.URL_ParamsMgmt);
        //}

        //private void MenuClick_DebugTools(object sender, RoutedEventArgs e)
        //{
        //    this.ViewModel.AppViewModel.NavigateTo(UrlDefines.URL_DebugTools);
        //}
    }
}