using Ctp0600P.Client.ViewModels;
using System;
using System.Windows;

namespace Ctp0600P.Client.UserControls.AGV
{
    /// <summary>
    /// AGVBingViewControl.xaml 的交互逻辑
    /// </summary>
    public partial class AGVBingPage : Window
    {
        public AGVBingPageViewModel _VM;

        public AGVBingPage(AGVBingPageViewModel vm)
        {
            this.DataContext = vm;
            _VM = vm;
            InitializeComponent();
            Topmost = true;
            WindowStyle = WindowStyle.ToolWindow;
            _VM.CloseWindowEvent += CloseThisWindow;
            this._VM.PackBarCode = "";
            this._VM.OutCode = "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            App._ActivityAGVBingPage = null;
            GC.Collect();
        }

        private async void Button_Click_BindingAGV(object sender, RoutedEventArgs e)
        {
            var result = await _VM.BindingAgv();
        }

        public void CloseThisWindow()
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                this.Close();
            });
        }
    }
}
