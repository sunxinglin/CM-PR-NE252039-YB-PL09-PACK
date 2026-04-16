using MPAssmebleRecipe.Logger.Interfaces;
using RogerTech.Common.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RogerTech.Common;
using Prism.Events;

namespace MPAssmebleRecipe.Apps.ViewModels.PackManage
{
    public class UcPackAddViewModel : BindableBase, IDialogAware,IDisposable
    {

      //  private readonly ILoggerHelper _logger;
        private string _packname;
        private string _packpn;
        private int _layertotal;

        public string Title => "添加Pack";
        private readonly IEventAggregator _eventAggregator;
        private SubscriptionToken _closeToken;
        public void Dispose()
        {
            // 取消订阅
            if (_closeToken != null)
            {
                _eventAggregator.GetEvent<CloseAllDialogsEvent>().Unsubscribe(_closeToken);
                _closeToken = null;
            }
        }
        public string PackName
        {
            get => _packname;
            set => SetProperty(ref _packname, value);
        }

        public string PackPn
        {
            get => _packpn;
            set {
                SetProperty(ref _packpn, value);
            }
        }
        public int LayerTotal
        {
            get=>_layertotal;
            set
            {
                SetProperty(ref _layertotal, value);
            }
        }

        public DelegateCommand ConfirmCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public event Action<IDialogResult> RequestClose;

        public UcPackAddViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _closeToken = _eventAggregator.GetEvent<CloseAllDialogsEvent>().Subscribe(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel)));

            _eventAggregator = eventAggregator;
            _closeToken = _eventAggregator.GetEvent<CloseAllDialogsEvent>().Subscribe(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel)));

            ConfirmCommand = new DelegateCommand(ExecuteConfirm, CanExecuteConfirm)
                .ObservesProperty(() => PackName)
                .ObservesProperty(() => PackPn)
                .ObservesProperty(() => LayerTotal);
            CancelCommand = new DelegateCommand(ExecuteCancel);
        }

        private bool CanExecuteConfirm()
        {
            return !string.IsNullOrWhiteSpace(PackName) &&
                   !string.IsNullOrWhiteSpace(PackPn) && LayerTotal > 0;
        }

        private void ExecuteConfirm()
        {
            try
            {
                var pack = new Template_Pack
                {
                    PackName = PackName.Trim(),
                    PackPn = PackPn.Trim(),
                    LayerTotal = LayerTotal,
                    PackUid = Guid.NewGuid().ToString(),
                    CreateTime = DateTime.Now
                };
                IDialogParameters parameters = new DialogParameters {
                    {"Pack",pack }
                };

                RequestClose?.Invoke(new DialogResult(ButtonResult.OK,parameters));
            }
            catch (Exception ex)
            {
              //  LogHelper.Errors("添加Pack失败");
                MessageBox.Show($"添加失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteCancel()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            Dispose();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
