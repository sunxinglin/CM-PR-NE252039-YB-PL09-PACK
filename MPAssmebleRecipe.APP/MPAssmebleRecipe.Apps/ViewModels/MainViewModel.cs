using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RogerTech.AuthService.Models;
using RogerTech.Common;
using RogerTech.Common.AuthService;
using RogerTech.Common.AuthService.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace MPAssmebleRecipe.Apps.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private string _title = "MES管理系统";
        private bool _isMenuOpen;
        private readonly IRegionManager _regionManager;
        private readonly IDialogService _dialogService;
        private readonly DispatcherTimer _autoLogoutTimer;
        private int _autoLogoutCountdown = 180;
        private string _softwareVersion = "v1.0.0";
        private string _additionalInfo = "正常运行中";
        private ObservableCollection<DeviceStatus> _deviceStatuses;
        private UserInfo _currentUser;

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

        private IEventAggregator _eventAggregator;

        public MainViewModel(IRegionManager regionManager, IDialogService dialogService, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _dialogService = dialogService;

            ToggleMenuCommand = new DelegateCommand(() => IsMenuOpen = true);
            HomeCommand = new DelegateCommand(NavigateToHome);
            MinimizeCommand = new DelegateCommand(MinimizeWindow);
            CloseCommand = new DelegateCommand(CloseWindow);
            SwitchUserCommand = new DelegateCommand(SwitchUser);
            LogoutCommand = new DelegateCommand(Logout);
            ShowLoginCommand = new DelegateCommand(ShowLoginDialog);
            InitializeMenu();

            // 初始化设备状态
            DbContext.GetInstance();
            DeviceStatuses = new ObservableCollection<DeviceStatus>();
            AppManager appManager = AppManager.GetInstance();
            if (appManager.bussness.BussnessDic.PlcServer.Connections != null)
            {
                foreach (var deviceStatus in appManager.bussness.BussnessDic.PlcServer.Connections.Select(item => new DeviceStatus()
                         {
                             DeviceName = item.IP,
                             IsConnected = item.Connected
                         }))
                {
                    DeviceStatuses.Add(deviceStatus);
                }
            }

            // 初始化自动注销计时器
            _autoLogoutTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _autoLogoutTimer.Tick += AutoLogoutTimer_Tick;
            _autoLogoutTimer.Start();
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<UserInfoEvent>().Publish(CurrentUser);
            GetOperationUserInfo();
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

        public UserInfo CurrentUser
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
            AppManager appManager = AppManager.GetInstance();
            if (appManager.bussness.BussnessDic.PlcServer.Connections != null)
            {
                DeviceStatuses.Clear();
                foreach (var item in appManager.bussness.BussnessDic.PlcServer.Connections)
                {
                    DeviceStatus deviceStatus = new DeviceStatus()
                    {
                        DeviceName = item.IP,
                        IsConnected = item.Connected
                    };
                    DeviceStatuses.Add(deviceStatus);
                }
            }
            if (appManager.UserInfo == null)
            {
                _autoLogoutCountdown = 0;
                RaisePropertyChanged(nameof(AutoLogoutCountdown));

                return;
            }
            else
            {
                Console.WriteLine();
            }
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
            _regionManager.RequestNavigate("ContentRegion", "LogView");
            IsMenuOpen = false;
        }

        private void MinimizeWindow()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void CloseWindow()
        {
            AppManager appManager = AppManager.GetInstance();
            if (appManager.UserInfo != null)
            {
                var result = MessageBox.Show("是否退出程序", "", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.CloseApp, "用户关闭软件");
                    Application.Current.Shutdown();
                }
                else
                {
                    return;
                }
            }
            return;
            Application.Current.Shutdown();
        }

        private void SwitchUser()
        {
            // 实现切换用户逻辑
        }

        private void Logout()
        {
            // 实现注销逻辑
            OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.OutLogin, "用户退出");
            CurrentUser = null;
            AppManager appManager = AppManager.GetInstance();
            appManager.UserInfo = null;
            _eventAggregator.GetEvent<UserInfoEvent>().Publish(CurrentUser);
        }

        private bool _loginFlag = true;
        private void ShowLoginDialog()
        {
            if (!_loginFlag)
            {
                _loginFlag = true;
                return;
            }
            _loginFlag = false;
            var parameters = new DialogParameters();
            _dialogService.ShowDialog("CardLoginDialog", parameters, result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    //ShowLoginCommand = new DelegateCommand(ShowLoginDialog);
                    // 处理登录成功后的逻辑
                    var user = result.Parameters.GetValue<User>("user");
                    // 更新UI或其他操作

                    UserService userService = new UserService();
                    AppManager appManager = AppManager.GetInstance();
                    appManager.UserInfo = userService.Login(user.UserName, user.PasswordHash);
                    CurrentUser = appManager.UserInfo;
                    _eventAggregator.GetEvent<UserInfoEvent>().Publish(CurrentUser);
                    OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Login, "用户登录");
                    ResetAutoLogoutTimer();

                    if (CurrentUser == null)
                    {
                        ShowLoginDialog();
                    }
                }
            });

        }

        // ... 现有代码 ...

        public class MenuItem
        {
            public string Title { get; set; }
            public string Icon { get; set; }

            public string ViewName { get; set; } // 关键：存储视图名称
            private ObservableCollection<MenuItem> _children = new ObservableCollection<MenuItem>();
            public ObservableCollection<MenuItem> Children
            {
                get => _children;
                set
                {
                    _children = value;
                    OnPropertyChanged(nameof(Children));

                }
            }
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private ObservableCollection<MenuItem> _menuItems;
        public ObservableCollection<MenuItem> MenuItems
        {
            get { return _menuItems; }
            set { SetProperty(ref _menuItems, value); }
        }

        private void InitializeMenu()
        {
            MenuItems = new ObservableCollection<MenuItem>
        {
            new MenuItem
            {
                Title = "交互日志",Icon = "FileDocument",ViewName = "LogView",
                Children = new ObservableCollection<MenuItem>
                {
                    new MenuItem { Title = "电芯配方下发", Icon = "Battery", ViewName = "UcRecipeManageView" },
                    new MenuItem { Title = "配方设置", Icon = "Settings", ViewName = "UcPackManageView" },
                    new MenuItem { Title = "电芯管理", Icon = "Battery", ViewName = "UcCurrentCellOrderView" },

                }
            },
            new MenuItem { Title = "MES设置", Icon = "ServerNetwork", ViewName = "MESCFGView" },
            new MenuItem { Title = "数据查询", Icon = "DatabaseSearch", ViewName = "UploadDataView" },
            new MenuItem { Title = "PLC变量监控", Icon = "PlcTagMonitor", ViewName = "PlcTagView" },
            new MenuItem { Title = "操作记录", Icon = "History", ViewName = "UcOperationView" },
            new MenuItem { Title = "用户管理", Icon = "AccountMultiple", ViewName = "AuthMainView" }
        };
        }

        private void NavigateToUserControl(string viewName)
        {
            if (string.IsNullOrEmpty(viewName)) return;

            // 清除区域内容并导航到新视图
            _regionManager.Regions["ContentRegion"].RemoveAll();
            IsMenuOpen = false;  // 关闭菜单
            _regionManager.RequestNavigate("ContentRegion", viewName);
        }


        private MenuItem _selectedMenuItem;

        public MenuItem SelectedMenuItem
        {
            get => _selectedMenuItem;
            set
            {
                if (SetProperty(ref _selectedMenuItem, value) && _selectedMenuItem != null)
                {
                    // 直接从当前选中的菜单项获取 ViewName
                    NavigateToUserControl(_selectedMenuItem.ViewName);
                }
            }
        }


        // ... 其他导航方法 ...

        public void GetOperationUserInfo()
        {
            _eventAggregator.GetEvent<UserInfoEvent>().Subscribe(user =>
            {
                OperationService.userInfo = user;
            });
        }

    }
}
