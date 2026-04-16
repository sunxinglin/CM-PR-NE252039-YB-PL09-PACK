using MPAssmebleRecipe.Apps.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace UserTest.ViewModels
{
    public class ConfirmDialogViewModel : BindableBase, IDialogAware,IDisposable
    {
        private string _title;
        private string _message;
        private readonly IEventAggregator _eventAggregator;
        private SubscriptionToken _closeToken;
        public void Dispose()
        {
            // È¡Ïû¶©ÔÄ
            if (_closeToken != null)
            {
                _eventAggregator.GetEvent<CloseAllDialogsEvent>().Unsubscribe(_closeToken);
                _closeToken = null;
            }
        }
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public event Action<IDialogResult> RequestClose;

        public DelegateCommand ConfirmCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public ConfirmDialogViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _closeToken = _eventAggregator.GetEvent<CloseAllDialogsEvent>().Subscribe(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel)));

            ConfirmCommand = new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(ButtonResult.OK)));
            CancelCommand = new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel)));
        }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            Dispose();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Title = parameters.GetValue<string>("title");
            Message = parameters.GetValue<string>("message");
        }
    }
}