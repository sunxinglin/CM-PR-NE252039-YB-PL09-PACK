using DryIoc;
using MPAssmebleRecipe.Logger.Interfaces;
//using MPAssmebleRecipe.Service.IService;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RogerTech.Common;
using RogerTech.Common.AuthService.Services;
using RogerTech.Common.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace MPAssmebleRecipe.Apps.ViewModels.CellManage
{
    /// <summary>
    /// Cell维护视图模型
    /// </summary>
    public class UcCellEditViewModel : BindableBase, IDialogAware
    {
       // private readonly IBlockService _blockService;
        private readonly ILoggerHelper _logger;
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
        public string Title => "维护Cell";

        private ObservableCollection<Template_Cell> _cellItems;
        public ObservableCollection<Template_Cell> CellItems
        {
            get { return _cellItems; }
            set { SetProperty(ref _cellItems, value); }
        }

        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        public event Action<IDialogResult> RequestClose;

        public void OnDialogOpened(IDialogParameters parameters)
        {
            int moduleid = parameters.GetValue<int>("ModuleId");
            List<Template_Cell> modules = DbContext.GetInstance().Queryable<Template_Cell>().Where(r=>r.ModuleId == moduleid).OrderBy(r=>r.CellIndex).ToList();
            if (modules == null || modules.Count <= 0)
                return;
            CellItems = new ObservableCollection<Template_Cell>(modules);
        }
        public bool CanCloseDialog()
        {
            return true;
        }
        public void OnDialogClosed()
        {
            Dispose();
        }

        public UcCellEditViewModel(ILoggerHelper logger, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _closeToken = _eventAggregator.GetEvent<CloseAllDialogsEvent>().Subscribe(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel)));

            _logger = logger;
            SaveCommand = new DelegateCommand(ExecuteSave, CanExecuteSave);
            CancelCommand = new DelegateCommand(ExecuteCancel);
        }

        private bool CanExecuteSave()
        {
            return CellItems != null && CellItems.Count > 0;
        }
        private void ExecuteSave()
        {
            try
            {
                if (CellItems == null || CellItems.Count == 0)
                {
                    MessageBox.Show("请先选择要维护的Cell信息！");
                    return;
                }
                var result = MessageBox.Show("确认保存Cell信息", "提示", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    var rowCount = DbContext.GetInstance().Updateable<Template_Cell>(CellItems).ExecuteCommand();
                    OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Delet, $"用户进行Cell信息维护,数量:{rowCount}");
                }
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
            }
            catch (Exception ex)
            {
                //LogHelper.Errors("保存Cell信息");
                MessageBox.Show($"保存Cell信息：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ExecuteCancel()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }
    }
}