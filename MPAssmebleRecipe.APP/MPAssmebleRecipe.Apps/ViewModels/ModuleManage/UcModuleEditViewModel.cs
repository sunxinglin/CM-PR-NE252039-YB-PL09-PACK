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

namespace MPAssmebleRecipe.Apps.ViewModels.ModuleManage
{
    /// <summary>
    /// Module维护视图模型
    /// </summary>
    public class UcModuleEditViewModel : BindableBase, IDialogAware
    {
       // private readonly IBlockService _blockService;
        private readonly ILoggerHelper _logger;

        public string Title => "维护Module";

        private ObservableCollection<Template_Module> _moduleItems;
        public ObservableCollection<Template_Module> ModuleItems
        {
            get { return _moduleItems; }
            set { SetProperty(ref _moduleItems, value); }
        }

        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        public event Action<IDialogResult> RequestClose;

        public void OnDialogOpened(IDialogParameters parameters)
        {
            string blockuid = parameters.GetValue<string>("BlockUid");
            List<Template_Module> modules = DbContext.GetInstance().Queryable<Template_Module>().Where(r=>r.ModuleUid == blockuid).OrderBy(r=>r.ModuleIndex).ToList();
            if (modules == null || modules.Count <= 0)
                return;
            ModuleItems = new ObservableCollection<Template_Module>(modules);
        }
        public bool CanCloseDialog()
        {
            return true;
        }
        public void OnDialogClosed()
        {

        }

        public UcModuleEditViewModel(ILoggerHelper logger)
        {
          //  _blockService = blockService;
            _logger = logger;
            SaveCommand = new DelegateCommand(ExecuteSave, CanExecuteSave);
            CancelCommand = new DelegateCommand(ExecuteCancel);
        }

        private bool CanExecuteSave()
        {
            return ModuleItems != null && ModuleItems.Count > 0;
        }
        private void ExecuteSave()
        {
            try
            {
                if (ModuleItems == null || ModuleItems.Count == 0)
                {
                    MessageBox.Show("请先选择要维护的Module信息！");
                    return;
                }
                var result = MessageBox.Show("确认保存Module信息", "提示", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    var rowCount = DbContext.GetInstance().Updateable<Template_Module>(ModuleItems).ExecuteCommand();
                    OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Delet, $"用户进行Module信息维护,数量:{rowCount}");
                }
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
            }
            catch (Exception ex)
            {
               // LogHelper.Errors("保存Module信息");
                MessageBox.Show($"保存Module信息：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ExecuteCancel()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }
    }
}