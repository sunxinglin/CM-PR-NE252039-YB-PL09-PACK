using Prism.Mvvm;
using Prism.Regions;
using System.Windows;
using Prism.Commands;
using Prism.Services;
using System.Windows.Input;
using Prism.Services.Dialogs;
using RogerTech.AuthService.Models;
using RogerTech.AuthService;
using RogerTech.Common;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows.Controls;
using System;

namespace UserTest.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "组件MES管理系统";
        private bool _isMenuOpen;
        private readonly IRegionManager _regionManager;
        private readonly IDialogService _dialogService;
        private readonly DispatcherTimer _autoLogoutTimer;
        private int _autoLogoutCountdown = 180;
        private string _softwareVersion = "v1.0.0";
        private string _additionalInfo = "正常运行中";
        private ObservableCollection<DeviceStatus> _deviceStatuses;
        private User _currentUser;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public bool IsMenuOpen
        {
            get => _isMenuOpen;
            set => SetProperty(ref _isMenuOpen, value);
        }

        public DelegateCommand ToggleMenuCommand { get; }
        public DelegateCommand HomeCommand { get; }
        public DelegateCommand MinimizeCommand { get; }
        public DelegateCommand CloseCommand { get; }
        public DelegateCommand SwitchUserCommand { get; }
        public DelegateCommand LogoutCommand { get; }
        public ICommand ShowLoginCommand { get; private set; }

        public MainWindowViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            _regionManager = regionManager;
            _dialogService = dialogService;

            ToggleMenuCommand = new DelegateCommand(() => IsMenuOpen = !IsMenuOpen);
            HomeCommand = new DelegateCommand(NavigateToHome);
            MinimizeCommand = new DelegateCommand(MinimizeWindow);
            CloseCommand = new DelegateCommand(CloseWindow);
            SwitchUserCommand = new DelegateCommand(SwitchUser);
            LogoutCommand = new DelegateCommand(Logout);
            ShowLoginCommand = new DelegateCommand(ShowLoginDialog);

            // 初始化设备状态
            DeviceStatuses = new ObservableCollection<DeviceStatus>
            {
                new DeviceStatus { DeviceName = "设备1", IsConnected = true },
                new DeviceStatus { DeviceName = "设备2", IsConnected = false }
            };
            InitializeMenu();
            // 初始化自动注销计时器
            _autoLogoutTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _autoLogoutTimer.Tick += AutoLogoutTimer_Tick;
            _autoLogoutTimer.Start();
        }

        public ObservableCollection<DeviceStatus> DeviceStatuses
        {
            get => _deviceStatuses;
            set => SetProperty(ref _deviceStatuses, value);
        }

        public string SoftwareVersion
        {
            get => _softwareVersion;
            set => SetProperty(ref _softwareVersion, value);
        }

        public string AdditionalInfo
        {
            get => _additionalInfo;
            set => SetProperty(ref _additionalInfo, value);
        }

        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public string AutoLogoutCountdown
        {
            get => $"自动注销: {_autoLogoutCountdown}s";
        }

        private void AutoLogoutTimer_Tick(object sender, EventArgs e)
        {
            _autoLogoutCountdown--;
            RaisePropertyChanged(nameof(AutoLogoutCountdown));

            if (_autoLogoutCountdown <= 0)
            {
                Logout();
            }
        }

        public void ResetAutoLogoutTimer()
        {
            _autoLogoutCountdown = 180;
            RaisePropertyChanged(nameof(AutoLogoutCountdown));
        }

        // 设备状态类
        public class DeviceStatus : BindableBase
        {
            private string _deviceName;
            private bool _isConnected;

            public string DeviceName
            {
                get => _deviceName;
                set => SetProperty(ref _deviceName, value);
            }

            public bool IsConnected
            {
                get => _isConnected;
                set => SetProperty(ref _isConnected, value);
            }
        }

        private void NavigateToHome()
        {
            _regionManager.RequestNavigate("ContentRegion", "HomeView");
            IsMenuOpen = false;
        }

        private void MinimizeWindow()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void CloseWindow()
        {
            Application.Current.Shutdown();
        }

        private void SwitchUser()
        {
            // 实现切换用户逻辑
        }

        private void Logout()
        {
            // 实现注销逻辑
        }

        private void ShowLoginDialog()
        {
            var parameters = new DialogParameters();
            _dialogService.ShowDialog("LoginDialog", parameters, result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    // 处理登录成功后的逻辑
                    var user = result.Parameters.GetValue<User>("user");
                    // 更新UI或其他操作

                    var item = DbContext.GetInstance().Queryable<User>().Where(x => x.UserName == user.UserName && x.PasswordHash == user.PasswordHash).First();
                    if (item != null)
                    {

                    }
                    else
                    {

                    }
                }
            });
        }

        // ... 现有代码 ...

        public class MenuItem
        {
            public string Title { get; set; }
            public string Icon { get; set; }
            public DelegateCommand Command { get; set; }
        }

        private ObservableCollection<MenuItem> _menuItems;
        public ObservableCollection<MenuItem> MenuItems
        {
            get { return _menuItems; }
            set { SetProperty(ref _menuItems, value); }
        }

        private void InitializeMenu() => MenuItems = new ObservableCollection<MenuItem>
        {
            new MenuItem { Title = "交互日志", Icon = "FileDocument", Command = new DelegateCommand(NavigateToInteractionLog) },
            new MenuItem { Title = "电芯配方下发", Icon = "Battery", Command = new DelegateCommand(NavigateToBatteryRecipe) },
            new MenuItem { Title = "转线配方下发", Icon = "TransitConnection", Command = new DelegateCommand(NavigateToLineRecipe) },
            new MenuItem { Title = "生产设置", Icon = "Settings", Command = new DelegateCommand(NavigateToProductionSettings) },
            new MenuItem { Title = "数据查询", Icon = "DatabaseSearch", Command = new DelegateCommand(NavigateToDataQuery) },
            new MenuItem { Title = "MES设置", Icon = "ServerNetwork", Command = new DelegateCommand(NavigateToMesSettings) },
            new MenuItem { Title = "用户管理", Icon = "AccountMultiple", Command = new DelegateCommand(NavigateToUserManagement) },
            new MenuItem { Title = "操作记录", Icon = "History", Command = new DelegateCommand(NavigateToOperationLog) }
        };
        private void NavigateToOperationLog()
        {
            //throw new NotImplementedException();
        }

        private void NavigateToUserManagement()
        {
            //throw new NotImplementedException();
        }

        private void NavigateToMesSettings()
        {
            //throw new NotImplementedException();
        }

        private void NavigateToDataQuery()
        {
            //throw new NotImplementedException();
        }

        private void NavigateToProductionSettings()
        {
            //throw new NotImplementedException();
        }

        private void NavigateToLineRecipe()
        {
            //throw new NotImplementedException();
        }

        // 导航方法
        private void NavigateToInteractionLog()
        {
            _regionManager.RequestNavigate("ContentRegion", "InteractionLogView");
        }

        private void NavigateToBatteryRecipe()
        {
            _regionManager.RequestNavigate("ContentRegion", "BatteryRecipeView");
        }

        // ... 其他导航方法 ...
    }
}
