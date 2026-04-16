using System;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Input;

using Ctp0600P.Client.ViewModels;

using Application = System.Windows.Forms.Application;

namespace Ctp0600P.Client.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private Timer timer;
        public LoginWindow(LoginPageViewModel vm)
        {
            InitializeComponent();
            //this.Password.Password = "123456";
            this.DataContext = vm;
            timer = new Timer(500);
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
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
            Application.ExitThread();
            Application.Exit();
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

