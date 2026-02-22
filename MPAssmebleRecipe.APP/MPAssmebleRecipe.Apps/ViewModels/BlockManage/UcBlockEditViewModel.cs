using DryIoc;
using MPAssmebleRecipe.Logger.Interfaces;
//using MPAssmebleRecipe.Service.IService;
using Prism.Commands;
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

namespace MPAssmebleRecipe.Apps.ViewModels.BlockManage
{
    /// <summary>
    /// Block维护视图模型
    /// </summary>
    public class UcBlockEditViewModel : BindableBase, IDialogAware
    {
       // private readonly IBlockService _blockService;
        private readonly ILoggerHelper _logger;

        public string Title => "维护Block";

        private ObservableCollection<Template_Block> _blockItems;
        public ObservableCollection<Template_Block> BlockItems
        {
            get { return _blockItems; }
            set { SetProperty(ref _blockItems, value); }
        }

        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        public event Action<IDialogResult> RequestClose;

        public void OnDialogOpened(IDialogParameters parameters)
        {
            int packid = parameters.GetValue<int>("PackId");
            List<Template_Block> blocks = DbContext.GetInstance().Queryable<Template_Block>().Where(r=>r.PackId == packid).ToList();
            if (blocks != null)
                blocks = blocks.OrderBy(p => p.BlockSequence).ThenBy(x => x.BlockIndex).ToList();
            if (blocks == null || blocks.Count <= 0)
                return;
            BlockItems = new ObservableCollection<Template_Block>(blocks);
        }
        public bool CanCloseDialog()
        {
            return true;
        }
        public void OnDialogClosed()
        {

        }

        public UcBlockEditViewModel(ILoggerHelper logger)
        {
          //  _blockService = blockService;
            _logger = logger;
            SaveCommand = new DelegateCommand(ExecuteSave, CanExecuteSave);
            CancelCommand = new DelegateCommand(ExecuteCancel);
        }

        private bool CanExecuteSave()
        {
            return BlockItems != null && BlockItems.Count > 0;
        }
        private void ExecuteSave()
        {
            try
            {
                if (BlockItems == null || BlockItems.Count == 0)
                {
                    MessageBox.Show("请先选择要维护的Block信息！");
                    return;
                }
                var result = MessageBox.Show("确认保存Block信息", "提示", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    var rowCount = DbContext.GetInstance().Updateable<Template_Block>(BlockItems).ExecuteCommand();
                    OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Delet, $"用户进行Block信息维护,数量:{rowCount}");
                }
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
            }
            catch (Exception ex)
            {
              // LogHelper.Errors("保存Block信息");
                MessageBox.Show($"保存Block信息：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ExecuteCancel()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }
    }
}