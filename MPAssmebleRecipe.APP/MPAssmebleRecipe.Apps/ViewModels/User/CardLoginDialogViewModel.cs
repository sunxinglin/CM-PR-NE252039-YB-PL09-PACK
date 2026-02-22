using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using DialogResult = Prism.Services.Dialogs.DialogResult;

namespace MPAssmebleRecipe.Apps.ViewModels
{
    public class CardLoginDialogViewModel : BindableBase, IDialogAware
    {
        private string _cardNumber;
        private readonly IDialogService _dialogService;
        public CardLoginDialogViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            CancelCommand = new DelegateCommand(Cancel);
            SwitchToPasswordLoginCommand = new DelegateCommand(SwitchToPasswordLogin);
            LoginCommand = new DelegateCommand(ExcuteLoginCommand);         
        }

        private void ExcuteLoginCommand()
        {
            RogerTech.AuthService.Models.User user = new RogerTech.AuthService.Models.User()
            {
                PasswordHash = CardNumber.Trim()
            };
            var parameters = new DialogParameters
            {
                { "user", user }
            };

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, parameters));
        }

        public string CardNumber
        {
            get => _cardNumber;
            set
            {
                SetProperty(ref _cardNumber, value);
                if (_cardNumber != null && _cardNumber.Contains('\n'))
                {
                    OnCardRead(CardNumber.Trim());
                }
            }
        }

        public DelegateCommand CancelCommand { get; }
        public DelegateCommand SwitchToPasswordLoginCommand { get; }
        public DelegateCommand LoginCommand { get; }
        public string Title => "刷卡登录";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

        }

        public void OnCardRead(string cardNumber)
        {

            CardNumber = cardNumber;
            if (CardNumber == null)
            {
                cardNumber = " ";
            }
            RogerTech.AuthService.Models.User user = new RogerTech.AuthService.Models.User()
            {
                PasswordHash = CardNumber.Trim()
            };
            var parameters = new DialogParameters
            {
                { "user", user }
            };

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, parameters));

        }

        private void Cancel()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        private void SwitchToPasswordLogin()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.None));
            _dialogService.ShowDialog("LoginDialog", null, null);
        }
    }
}
