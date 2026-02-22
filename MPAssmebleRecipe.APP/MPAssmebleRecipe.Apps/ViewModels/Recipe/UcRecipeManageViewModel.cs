using MPAssmebleRecipe.Apps.Common;
using MPAssmebleRecipe.Logger.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RogerTech.BussnessCore;
using RogerTech.Common;
using RogerTech.Common.AuthService.Services;
using RogerTech.Common.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MPAssmebleRecipe.Apps.ViewModels.Recipe
{
    public class UcRecipeManageViewModel : ViewModelBase
    {
        private readonly ILoggerHelper _logger;
        private readonly IDialogService _dialogService;      // 对话框服务

        /// <summary>
        /// 正常配方
        /// </summary>
        private ObservableCollection<Recipe_Temp> recipeTemps = new ObservableCollection<Recipe_Temp>();
        public ObservableCollection<Recipe_Temp> RecipeTemps
        {
            get => recipeTemps; 
            set => SetProperty(ref recipeTemps, value);
        }
        /// <summary>
        /// 开拉配方
        /// </summary>
        private ObservableCollection<Recipe_Temp> recipeTempsKara = new ObservableCollection<Recipe_Temp>();
        public ObservableCollection<Recipe_Temp> RecipeTempsKara
        {
            get => recipeTempsKara; 
            set => SetProperty(ref recipeTempsKara, value);
        }
        /// <summary>
        /// 收拉配方
        /// </summary>
        private ObservableCollection<Recipe_Temp> recipeTempsFinish = new ObservableCollection<Recipe_Temp>();
        public ObservableCollection<Recipe_Temp> RecipeTempsFinish
        { 
            get => recipeTempsFinish;
            set => SetProperty(ref recipeTempsFinish, value);
        }

        /// <summary>
        /// 选中正常配方
        /// </summary>
        private Recipe_Temp _SelectedRecipe;
        public Recipe_Temp SelectedRecipe
        {
            get => _SelectedRecipe; 
            set => SetProperty(ref _SelectedRecipe, value);
        }

        public DelegateCommand RefshCommand { get; set; }
        public DelegateCommand RecipeAddCommand { get; set; }
        public DelegateCommand RecipeInsertCommand { get; set; }
        public DelegateCommand RecipeClearCommand { get; set; }

        public UcRecipeManageViewModel(ILoggerHelper logger, IDialogService dialogService) : base(logger)
        {
            _logger = logger;
            _dialogService = dialogService;
            InitializeCommands();
            LoadRecipe();
        }
        private void InitializeCommands()
        {
            RefshCommand = new DelegateCommand(ExcuteRefshCommand);
            RecipeAddCommand = new DelegateCommand(ExcuteRecipeAddCommand);
            RecipeInsertCommand = new DelegateCommand(ExcuteRecipeInsertCommand);
            RecipeClearCommand = new DelegateCommand(ExcuteRecipeClearCommand);
        }
        private void LoadRecipe()
        {
            var items_temp1 = DbContext.GetInstance().Queryable<Recipe_Temp>().Where(r => r.IsEnable && r.RecipeType == RecipeTypeEnum.开拉).OrderBy(r => r.RecipeIndex).ToList();
            var items_temp2 = DbContext.GetInstance().Queryable<Recipe_Temp>().Where(r => r.IsEnable && r.RecipeType == RecipeTypeEnum.正常).OrderBy(r => r.RecipeIndex).Take(25).ToList();
            var items_temp3 = DbContext.GetInstance().Queryable<Recipe_Temp>().Where(r => r.IsEnable && r.RecipeType == RecipeTypeEnum.收拉).OrderBy(r => r.RecipeIndex).ToList();
            RecipeTempsKara = new ObservableCollection<Recipe_Temp>(items_temp1);
            RecipeTemps = new ObservableCollection<Recipe_Temp>(items_temp2);
            RecipeTempsFinish = new ObservableCollection<Recipe_Temp>(items_temp3);

            //Application.Current.Dispatcher.Invoke(() =>
            //{
            //    RecipeTempsKara = new ObservableCollection<Recipe_Temp>(items_temp1);
            //    RaisePropertyChanged(nameof(RecipeTempsKara));
            //    RecipeTemps = new ObservableCollection<Recipe_Temp>(items_temp2);
            //    RaisePropertyChanged(nameof(RecipeTemps));
            //    RecipeTempsFinish = new ObservableCollection<Recipe_Temp>(items_temp3);
            //    RaisePropertyChanged(nameof(RecipeTempsFinish));
            //});
        }
        /// <summary>
        /// 配方刷新
        /// </summary>
        private void ExcuteRefshCommand()
        {
            LoadRecipe();
        }
        /// <summary>
        /// 配方下发
        /// </summary>
        private void ExcuteRecipeAddCommand()
        {
            try
            {
                bool issuekara = !(RecipeTempsKara.Count > 0 || RecipeTempsFinish.Count > 0);
                IDialogParameters dialogParameters = new DialogParameters()
                {
                    { "RecipeType", 1},
                    { "InsertIndex", 0},
                    { "IssueKara", issuekara}
                };
                _dialogService.ShowDialog("UcRecipeAddView", dialogParameters, result =>
                {
                    if (result.Result == ButtonResult.OK)
                    {
                        LoadRecipe();
                    }
                });
            }
            catch (Exception ex)
            {
                //LogHelper.Errors("配方下发失败");
                MessageBox.Show("配方下发失败：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 选中行下方插入配方
        /// </summary>
        private void ExcuteRecipeInsertCommand()
        {
            try
            {
                if (SelectedRecipe == null) return;
                bool issuekara = !(RecipeTempsKara.Count > 0 || RecipeTempsFinish.Count > 0);
                IDialogParameters dialogParameters = new DialogParameters()
                {
                    { "RecipeType", 2},
                    { "InsertIndex", SelectedRecipe.RecipeIndex},
                    { "IssueKara", issuekara}
                };
                _dialogService.ShowDialog("UcRecipeAddView", dialogParameters, result =>
                {
                    if (result.Result == ButtonResult.OK)
                    {
                        LoadRecipe();
                    }
                });
            }
            catch (Exception ex)
            {
               // LogHelper.Errors("选中行下方插入配方失败");
                MessageBox.Show("选中行下方插入配方失败：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 配方清空
        /// </summary>
        private void ExcuteRecipeClearCommand()
        {
            var result = MessageBox.Show("确认是否清空所有配方", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var rowCount = DbContext.GetInstance().Updateable<Recipe_Temp>().SetColumns(r => r.IsEnable == true).Where(s => s.IsEnable == false).ExecuteCommand();
                OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Delet, $"用户清空所有配方,数量:{rowCount}");
                LoadRecipe();
            }
        }
    }
}