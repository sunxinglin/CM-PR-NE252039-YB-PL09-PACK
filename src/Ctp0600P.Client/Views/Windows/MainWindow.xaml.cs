using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Ctp0600P.Client.CommonEntity;
using Ctp0600P.Client.ViewModels;
using Ctp0600P.Client.Views;

using Microsoft.Extensions.DependencyInjection;

using Yee.Common.Library.CommonEnum;

namespace Ctp0600P.Client;

public partial class MainWindow : Window
{
    // Windows API：获取鼠标在屏幕上的绝对坐标
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool GetCursorPos(out POINT lpPoint);

    POINT curPoint;
    public AppViewModel AppVM { get; }

    private IServiceProvider _sp { get; set; }

    public MainWindow(AppViewModel appvm, IServiceProvider sp)
    {
        InitializeComponent();

        _sp = sp;
        this.AppVM = appvm;
        this.DataContext = this.AppVM;

        if (App._StepStationSetting != null && !App._StepStationSetting.OccupyTaskbar)
        {
            this.MaxHeight = SystemParameters.WorkArea.Height;
            this.MaxWidth = SystemParameters.WorkArea.Width;
        }

        App.DoingWorkTime = DateTime.Now;
        
        // 监听page变化，控制MainWindow中Frame的跳转
        this.AppVM.CurrentPage.Subscribe(page =>
        {
            if (page != null)
            {
                this.navWin.Navigate(page);
            }
        });

        // 监听鼠标移动，记录鼠标在屏幕上的坐标
        GetCursorPos(out curPoint);
        Task.Factory.StartNew(async () =>
        {
            while (App._StepStationSetting.StepType != StepTypeEnum.自动站)
            {
                await Task.Delay(100);
                POINT tmpPoint = new();
                GetCursorPos(out tmpPoint);
                if (tmpPoint.X != curPoint.X || tmpPoint.Y != curPoint.Y)
                {
                    curPoint = tmpPoint;
                    App.DoingWorkTime = DateTime.Now;
                }
            }
        });

    }

    private void Window_Closed(object sender, EventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        var res = MessageBox.Show("是否真的要退出？", "退出程序确认", MessageBoxButton.YesNo);
    
        if (res.Equals(MessageBoxResult.Yes))
        {
            using var scope = _sp.CreateScope();
            var sp = scope.ServiceProvider;
            var realVM = sp.GetRequiredService<RealtimePageViewModel>();
            realVM.DisableBoltGuns();
        }
        else
        {
            e.Cancel = true;
        }
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            // 启用窗口拖动模式
            this.DragMove();
        }
    }

    private void Minimize_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void MenuClick_History(object sender, RoutedEventArgs e)
    {
        this.AppVM.NavigateTo(UrlDefines.URL_History);
    }

    private void MenuClick_Realtime(object sender, RoutedEventArgs e)
    {
        switch (App._StepStationSetting.StepType)
        {
            case StepTypeEnum.自动站:
                this.AppVM.NavigateTo(UrlDefines.URL_Realtime_Auto);
                break;
            case StepTypeEnum.线内人工站:
                this.AppVM.NavigateTo(UrlDefines.URL_Realtime);
                break;
            case StepTypeEnum.线外人工站:
                this.AppVM.NavigateTo(UrlDefines.URL_Realtime_OutLine);
                break;
            case StepTypeEnum.模组数量监控站:
                this.AppVM.NavigateTo(UrlDefines.URL_Realtime_OutLine);
                break;
        }
    }

    private void MenuClick_UserMgmt(object sender, RoutedEventArgs e)
    {
        this.AppVM.NavigateTo(UrlDefines.URL_UserMgmt);
    }

    private void MenuClick_ParamsMgmt(object sender, RoutedEventArgs e)
    {
        this.AppVM.NavigateTo(UrlDefines.URL_ParamsMgmt);
    }

    private void MenuClick_DebugTools(object sender, RoutedEventArgs e)
    {
        this.AppVM.NavigateTo(UrlDefines.URL_DebugTools);
    }

    private void MenuClick_HisDatas(object sender, RoutedEventArgs e)
    {
        this.AppVM.NavigateTo(UrlDefines.URL_HisDatas);
    }

    private void ToggleMenu_Click(object sender, RoutedEventArgs e)
    {
        MainDrawer.IsLeftDrawerOpen = !MainDrawer.IsLeftDrawerOpen;
    }

    private void Home_Click(object sender, RoutedEventArgs e)
    {
        this.AppVM.NavigateTo(UrlDefines.URL_Realtime); 
    }
}