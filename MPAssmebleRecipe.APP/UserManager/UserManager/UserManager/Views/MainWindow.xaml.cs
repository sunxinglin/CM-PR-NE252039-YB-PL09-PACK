using Prism.Regions;
using RogerTech.AuthService;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using RogerTech.AuthService.Models;
using Menu = RogerTech.AuthService.Models.Menu;
using System.Windows.Input;
using System;
using UserTest.ViewModels;

namespace UserTest.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IRegionManager region)
        {
            InitializeComponent();

            //region.RegisterViewWithRegion("ContentRegion", "");
            region.RegisterViewWithRegion("ContentRegion", "AuthMainView");
        }


        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 确保点击的是窗口顶部区域
            if (e.GetPosition(this).Y <= 40)  // 顶部DockPanel的高度
            {
                this.DragMove();
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // 重置自动登出计时器（如果有的话）
            if (DataContext is MainWindowViewModel viewModel1)
            {
                viewModel1.ResetAutoLogoutTimer();
            }
            // ESC键退出应用
            if (e.Key == Key.Escape)
            {
                Application.Current.Shutdown();
                e.Handled = true;
            }

            // Ctrl + M 最小化窗口
            if (e.Key == Key.M && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                this.WindowState = WindowState.Minimized;
                e.Handled = true;
            }

            // Ctrl + H 显示/隐藏菜单
            if (e.Key == Key.H && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (DataContext is MainWindowViewModel viewModel)
                {
                    viewModel.IsMenuOpen = !viewModel.IsMenuOpen;
                }
                e.Handled = true;
            }
        }

        private DateTime _lastMouseMove = DateTime.Now;
        private const int MouseMoveThreshold = 100; // 毫秒

        private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // 防止过于频繁的鼠标移动事件处理
            if ((DateTime.Now - _lastMouseMove).TotalMilliseconds < MouseMoveThreshold)
            {
                return;
            }

            _lastMouseMove = DateTime.Now;

            // 重置自动登出计时器（如果有的话）
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.ResetAutoLogoutTimer();
            }
        }
    }
}
