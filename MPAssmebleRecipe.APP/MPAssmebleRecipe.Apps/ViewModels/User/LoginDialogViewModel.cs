using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RogerTech.AuthService.Models;
using System;
using System.Configuration;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MPAssmebleRecipe.Apps.ViewModels
{
    public class LoginDialogViewModel : BindableBase, IDialogAware
    {
        private string _username;
        private bool _isPasswordLoginVisible;
        private bool _isCardLoginVisible;
        private bool _isSwitchVisible;
        private string _switchButtonText;
        private readonly DispatcherTimer _cardReaderTimer;
        private readonly string _loginMode;

        public string Title => "用户登录";

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public bool IsPasswordLoginVisible
        {
            get => _isPasswordLoginVisible;
            set => SetProperty(ref _isPasswordLoginVisible, value);
        }

        public bool IsCardLoginVisible
        {
            get => _isCardLoginVisible;
            set => SetProperty(ref _isCardLoginVisible, value);
        }

        public bool IsSwitchVisible
        {
            get => _isSwitchVisible;
            set => SetProperty(ref _isSwitchVisible, value);
        }

        public string SwitchButtonText
        {
            get => _switchButtonText;
            set => SetProperty(ref _switchButtonText, value);
        }

        public DelegateCommand<PasswordBox> LoginCommand { get; }
        public DelegateCommand SwitchLoginModeCommand { get; }

        public event Action<IDialogResult> RequestClose;

        public LoginDialogViewModel()
        {
            _loginMode = ConfigurationManager.AppSettings["LoginMode"];
            _loginMode = "Both";
            LoginCommand = new DelegateCommand<PasswordBox>(ExecuteLogin);
            SwitchLoginModeCommand = new DelegateCommand(SwitchLoginMode);

            // 初始化刷卡读取定时器
            _cardReaderTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _cardReaderTimer.Tick += CardReaderTimer_Tick;

            InitializeLoginMode();
        }

        private void InitializeLoginMode()
        {
            switch (_loginMode)
            {
                case "Both":
                    IsPasswordLoginVisible = true;
                    IsCardLoginVisible = false;
                    IsSwitchVisible = true;
                    SwitchButtonText = "切换到刷卡登录";
                    break;
                case "Card":
                    IsPasswordLoginVisible = false;
                    IsCardLoginVisible = true;
                    IsSwitchVisible = false;
                    StartCardReading();
                    break;
                case "Password":
                    IsPasswordLoginVisible = true;
                    IsCardLoginVisible = false;
                    IsSwitchVisible = false;
                    break;
            }
        }

        private void ExecuteLogin(PasswordBox passwordBox)
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(passwordBox?.Password))
            {
                // 显示错误提示
                return;
            }
            User user = new User()
            {
                UserName = Username,
                PasswordHash = passwordBox.Password
            };
            // TODO: 实现登录验证逻辑
            var parameters = new DialogParameters
            {
                { "user", user },
            };

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, parameters));
        }

        private void SwitchLoginMode()
        {
            IsPasswordLoginVisible = !IsPasswordLoginVisible;
            IsCardLoginVisible = !IsCardLoginVisible;
            SwitchButtonText = IsPasswordLoginVisible ? "切换到刷卡登录" : "切换到密码登录";

            if (IsCardLoginVisible)
            {
                StartCardReading();
            }
            else
            {
                StopCardReading();
            }
        }

        private void StartCardReading()
        {
            // TODO: 初始化串口
            _cardReaderTimer.Start();
        }

        private void StopCardReading()
        {
            _cardReaderTimer.Stop();
            // TODO: 关闭串口
        }

        private void CardReaderTimer_Tick(object sender, EventArgs e)
        {
            // TODO: 读取卡号
            // 如果读取到卡号，执行登录
            // var parameters = new DialogParameters
            // {
            //     { "cardNo", cardNo }
            // };
            // RequestClose?.Invoke(new DialogResult(ButtonResult.OK, parameters));
        }

        public bool CanCloseDialog() => true;

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }

        public void OnDialogClosed()
        {
            StopCardReading();
        }
    }
} 