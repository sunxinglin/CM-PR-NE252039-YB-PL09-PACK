using Ctp0600P.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Ctp0600P.Client.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private System.Timers.Timer timer;
        public LoginWindow(LoginPageViewModel vm)
        {
            InitializeComponent();
            //this.Password.Password = "123456";
            this.DataContext = vm;
            timer = new System.Timers.Timer(500);
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => CardNo.Clear()));
            timer.Stop();
            timer.Enabled = false;
        }

        private void MoveWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            //Application.Current.Shutdown();
            System.Windows.Forms.Application.ExitThread();
            System.Windows.Forms.Application.Exit();
            Process.GetCurrentProcess().Kill();
            Environment.Exit(0);
        }
        
        private void CardNo_PasswordChanged(object sender, RoutedEventArgs e)
        {
            //timer.Enabled = true;
            //timer.Start();
        }

    }
}

