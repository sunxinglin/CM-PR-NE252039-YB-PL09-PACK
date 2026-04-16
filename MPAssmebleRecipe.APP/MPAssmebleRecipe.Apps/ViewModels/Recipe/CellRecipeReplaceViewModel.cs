using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.FormulaExpressions;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RogerTech.AuthService;
using RogerTech.AuthService.Models;
using RogerTech.Common;
using RogerTech.Common.AuthService.Services;
using RogerTech.Common.Models;
using RogerTech.Tool;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace MPAssmebleRecipe.Apps.ViewModels.Recipe
{
    public class CellRecipeReplaceViewModel : BindableBase, IDialogAware
    {
        private readonly RogerTech.AuthService.AuthService _authService;
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

        private string _CellSN_Old;
        public string CellSN_Old
        {
            get => _CellSN_Old;
            set => SetProperty(ref _CellSN_Old, value);
        }

        private string _CellSN_New = string.Empty;
        public string CellSN_New
        {
            get => _CellSN_New;
            set => SetProperty(ref _CellSN_New, value);
        }
        public string Title => "配方电芯码替换";
        private string StationName = "A";
        private string CurrentTable = "production_recipea";

        public event Action<IDialogResult> RequestClose;

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public CellRecipeReplaceViewModel(AuthService authService, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _closeToken = _eventAggregator.GetEvent<CloseAllDialogsEvent>().Subscribe(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel)));

            _authService = authService;
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel)));
        }
        public void OnDialogOpened(IDialogParameters parameters)
        {
            StationName = parameters.GetValue<string>("StationName");
            CurrentTable = "production_recipe" + StationName.ToLower();
        }
        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { Dispose(); }

        private void Save()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CellSN_Old))
                {
                    MessageBox.Show("请输入旧电芯条码", "提示");
                    return;
                }
                if (string.IsNullOrWhiteSpace(CellSN_New))
                {
                    MessageBox.Show("请输入新电芯条码", "提示");
                    return;
                }
                if (CellSN_New.ToUpper() == CellSN_Old.ToUpper())
                {
                    MessageBox.Show("新旧码不能一样", "提示");
                    return;
                }
                List<Production_Recipe> production_s = DbContext.GetInstance().Queryable<Production_Recipe>().AS(CurrentTable).Where(r => r.CellSn == CellSN_Old).ToList();
                if (production_s == null || production_s.Count == 0)
                {
                    MessageBox.Show("未查找到旧电芯码的记录！", "提示");
                    return;
                }
                foreach (var item in production_s)
                {
                    item.CellSn = CellSN_New;
                }
                DbContext.GetInstance().Updateable<Production_Recipe>(production_s).AS(CurrentTable).ExecuteCommand();
                OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Delet, $"用户进行配方电芯码信息替换,旧码:{CellSN_Old}，新码:{CellSN_New}");
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"电芯码替换异常：{ex.Message}", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
