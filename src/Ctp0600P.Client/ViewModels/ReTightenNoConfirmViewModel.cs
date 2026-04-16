using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Ctp0600P.Client.Views.PopupPages;

using FutureTech.Mvvm;

using Microsoft.Extensions.DependencyInjection;

namespace Ctp0600P.Client.ViewModels
{
    public class ReTightenNoConfirmViewModel : ViewModelBase
    {
        private readonly IServiceScopeFactory _scf;
        public ReTightenNoConfirmViewModel(IServiceScopeFactory scf)
        {
            _scf = scf;

            this.NumberClick = new AsyncRelayCommand<Button>(async number =>
            {
                if (number.Content.ToString() == "清除")
                {
                    UserInput = String.Empty;
                    return;
                }
                if (number.Content.ToString() == "退格" && !string.IsNullOrEmpty(UserInput))
                {
                    UserInput = UserInput.Substring(0, UserInput.Length - 1);
                    return;
                }
                if (number.Content.ToString() == "确定")
                {
                    if (UserInput != TightenNo)
                    {
                        ErrorMessage = $"输入的补拧螺丝序号[{UserInput}]不匹配";
                        return;
                    }

                    if (this.Action == null)
                    {
                        ErrorMessage = $"待执行事件为空";
                        return;
                    }

                    UserInput = string.Empty;

                    using var scope = _scf.CreateScope();
                    var sp = scope.ServiceProvider;
                    var confirmPage = sp.GetRequiredService<ReTightenNoConfirmPage>();
                    confirmPage.Visibility = Visibility.Hidden;

                    Action();

                    return;
                }

                UserInput += number.Content.ToString();

            }, o => true);
        }

        public string TightenNo { get; set; }

        public Action Action { get; set; }

        private string _UserInput = string.Empty;
        public string UserInput
        {
            get => _UserInput;
            set
            {
                if (this._UserInput != value)
                {
                    _UserInput = value;
                    this.OnPropertyChanged(nameof(UserInput));
                }
            }
        }

        private string _ErrorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _ErrorMessage;
            set
            {
                if (this._ErrorMessage != value)
                {
                    _ErrorMessage = value;
                    this.OnPropertyChanged(nameof(ErrorMessage));
                }
            }
        }

        public ICommand NumberClick { get; }
    }
}
