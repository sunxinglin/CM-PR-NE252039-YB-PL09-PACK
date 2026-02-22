using Ctp0600P.Client.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace Ctp0600P.Client.Views.Windows
{
    /// <summary>
    /// CheckPowerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CheckPowerPage : Window
    {
        private System.Timers.Timer timer;

        public CheckPowerPage(CheckPowerViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
            this.CardNo.Focus();
            Topmost = true;
            timer = new System.Timers.Timer(3000);
            timer.AutoReset = true;
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.CardNo.Focus();
            Close();
            this.CardNo.Password = string.Empty;

        }

        public void OpenCheck()
        {
            this.ShowDialog();
        }

        private void win_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CardNo_PasswordChanged(object sender, RoutedEventArgs e)
        {

        }

        private void win_Loaded(object sender, RoutedEventArgs e)
        {
            this.CardNo.Focus();
            Topmost = true;
        }
    }
}
