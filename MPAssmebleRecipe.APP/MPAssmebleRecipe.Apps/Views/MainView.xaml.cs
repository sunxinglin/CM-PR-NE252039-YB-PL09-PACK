using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MPAssmebleRecipe.Apps.ViewModels;
using Prism.Regions;
using RogerTech.Tool;
using Menu = RogerTech.AuthService.Models.Menu;

namespace MPAssmebleRecipe.Apps.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        private MainViewModel _vm;
        IRegionManager regionManager;
        double x, y;
        public MainView(IRegionManager region)
        {
            InitializeComponent();

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);
            regionManager = region;
            _vm = (MainViewModel)DataContext;

            this.Loaded += OnLoaded;
            this.Closing += OnClosing;


            //注册菜单
            RegistMenu();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Console.WriteLine();
        }

        private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void RegistMenu()
        {
            List<Menu> menus = new List<Menu>();
            menus = new List<Menu>()
            {
               // new Menu(){ Page = "MainWindow", SubPage ="LogView" ,ElementName =""},
                //new Menu(){ Page = "MainWindow", SubPage ="CellRecipeView" ,ElementName =""},
                //new Menu(){ Page = "MainWindow", SubPage ="ModuleRecipeView" ,ElementName =""},
                //new Menu(){ Page = "MainWindow", SubPage ="ProductSettingView" ,ElementName =""},
                //new Menu(){ Page = "MainWindow", SubPage ="DataQueryView" ,ElementName =""},
                //new Menu(){ Page = "MainWindow", SubPage ="MesSettingView" ,ElementName =""},
                //new Menu(){ Page = "MainWindow", SubPage ="UserView" ,ElementName =""},
                //new Menu(){ Page = "MainWindow", SubPage ="OperationLogView" ,ElementName =""}
            };
            RogerTech.AuthService.AuthService authService = new RogerTech.AuthService.AuthService();
            foreach (var item in menus)
            {
                authService.AddMenu(item);
            }
        }
        private void OnLoaded(object sender, RoutedEventArgs e1)
        {
            //_vm.LoadModules.Execute();
            regionManager.Regions["ContentRegion"].RequestNavigate("LogView");

            MouseMove += new MouseEventHandler((s, e) =>
            {
                if (e.GetPosition(this).X - x > 10 || e.GetPosition(this).Y - y > 10)
                {
                    x = e.GetPosition(this).X;
                    y = e.GetPosition(this).Y;

                    _vm.ResetAutoLogoutTimer();
                }
            });
            KeyDown += new KeyEventHandler((s, e) =>
            {
                _vm.ResetAutoLogoutTimer();
            });
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 确保点击的是窗口顶部区域
            if (e.GetPosition(this).Y <= 40 && e.LeftButton == MouseButtonState.Pressed)  // 顶部DockPanel的高度
            {
                this.DragMove();
            }
        }


        private DateTime _lastMouseMove = DateTime.Now;

        
        private const int MouseMoveThreshold = 100; // 毫秒


    }
}
