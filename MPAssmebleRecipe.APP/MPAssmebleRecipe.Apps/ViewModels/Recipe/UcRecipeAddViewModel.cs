using MPAssmebleRecipe.Logger.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RogerTech.AuthService;
using RogerTech.BussnessCore;
using RogerTech.Common;
using RogerTech.Common.AuthService.Services;
using RogerTech.Common.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace MPAssmebleRecipe.Apps.ViewModels.Recipe
{
    public class UcRecipeAddViewModel : BindableBase, IDialogAware
    {
        private readonly ILoggerHelper _logger;
        public event Action<IDialogResult> RequestClose;

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; } = "添加配方";

        /// <summary>
        /// 配方添加类型
        /// 1正常[队尾]，2插入[选中行后面]
        /// </summary>
        public int RecipeTypes { get; set; } = 1;
        /// <summary>
        /// 插入序号
        /// </summary>
        public int InsertIndex { get; set; } = 0;
        /// <summary>
        /// 是否需要下发开拉和收拉队列，true需要，false不需要
        /// </summary>
        public bool InsertKara { get; set; } = false;

        /// <summary>
        /// 投产数量
        /// </summary>
        private int _WorkCount = 1;
        public int WorkCount
        {
            get { return _WorkCount; }
            set { SetProperty(ref _WorkCount, value); }
        }
        /// <summary>
        /// 层号
        /// </summary>
        private int _layernum = 0;
        public int LayerNum
        {
            get { return _layernum; }
            set { SetProperty(ref _layernum, value); }
        }
        /// <summary>
        /// Pack料号
        /// </summary>
        private string _packpn;
        public string PackPN
        {
            get { return _packpn; }
            set { SetProperty(ref _packpn, value); }
        }

        /// <summary>
        /// 配方
        /// </summary>
        private ObservableCollection<Recipe_Temp> _recipeTempss = new ObservableCollection<Recipe_Temp>();
        public ObservableCollection<Recipe_Temp> RecipeTempss
        {
            get { return _recipeTempss; }
            set { SetProperty(ref _recipeTempss, value); }
        }

        /// <summary>
        /// Pack集合
        /// </summary>
        //private ObservableCollection<Template_Pack> _lspackpn = new ObservableCollection<Template_Pack>();
        //public ObservableCollection<Template_Pack> lsPack
        //{
        //    get => _lspackpn;
        //    set => SetProperty(ref _lspackpn, value);
        //}

        /// <summary>
        /// 当前选中Pack
        /// </summary>
        //private Template_Pack _currentpack;
        //public Template_Pack CurrentPack
        //{
        //    get => _currentpack;
        //    set => SetProperty(ref _currentpack, value);
        //}

        public DelegateCommand RecipeLoadCommand { get; set; }
        public DelegateCommand RecipeSaveCommand { get; set; }

        public bool CanCloseDialog()
        {
            return true;
        }
        public void OnDialogClosed() { }
        public void OnDialogOpened(IDialogParameters parameters)
        {
            InsertIndex = parameters.GetValue<int>("InsertIndex");
            RecipeTypes = parameters.GetValue<int>("RecipeType");
            InsertKara = parameters.GetValue<bool>("IssueKara");
            switch (RecipeTypes)
            {
                case 2:
                    Title = "选中行后面插入配方";
                    break;
                default:
                    Title = "配方下发";
                    InsertIndex = DbContext.GetInstance().Queryable<Recipe_Temp>().Where(r => r.IsEnable).Max(r => r.RecipeIndex);
                    break;
            }
        }
        public UcRecipeAddViewModel(ILoggerHelper logger)
        {
            _logger = logger;
            // 异步初始化数据
            //InitializeDataAsync();
            //List<Template_Pack> template_s = DbContext.GetInstance().Queryable<Template_Pack>().OrderBy(r => r.Id).ToList();
            //lsPack = new ObservableCollection<Template_Pack>(template_s);
            RecipeLoadCommand = new DelegateCommand(ExcuteRecipeLoadCommand);
            RecipeSaveCommand = new DelegateCommand(ExcuteRecipeSaveCommand);
        }
        private async void InitializeDataAsync()
        {
            try
            {
                // 异步查询数据库
                List<Template_Pack> template_s = DbContext.GetInstance().Queryable<Template_Pack>().OrderBy(r => r.Id).ToList();
                // 在UI线程更新ObservableCollection
                Application.Current.Dispatcher.Invoke(() =>
                {
                    //lsPack = new ObservableCollection<Template_Pack>(template_s);
                    //RaisePropertyChanged(nameof(lsPack));
                });
            }
            catch (Exception ex)
            {
                _logger.Error($"初始化Pack数据失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 预览
        /// </summary>
        private void ExcuteRecipeLoadCommand()
        {
            RecipeTempss = new ObservableCollection<Recipe_Temp>();
            //if (CurrentPack == null || WorkCount <= 0)
            //    return;
            if (string.IsNullOrEmpty(PackPN) || WorkCount <= 0)
                return;
            Template_Pack template_Pack = DbContext.GetInstance().Queryable<Template_Pack>().Where(r => r.PackPn == PackPN).First();
            if(template_Pack == null)
            {
                MessageBox.Show("请先在[配方设置]中添加Pack信息", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            List<Template_Block> template_Blocks = new List<Template_Block>();
            if (LayerNum == 0)
                template_Blocks = DbContext.GetInstance().Queryable<Template_Block>().Where(r => r.PackId == template_Pack.Id).ToList();
            else
                template_Blocks = DbContext.GetInstance().Queryable<Template_Block>().Where(r => r.PackId == template_Pack.Id && r.LayerNum == LayerNum).ToList();
            if (template_Blocks == null || template_Blocks.Count == 0)
            {
                MessageBox.Show("请先在[配方设置]中添加Block信息", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            template_Blocks = template_Blocks.OrderBy(p => p.BlockSequence).ThenBy(s => s.BlockIndex).ToList();
            int inde = InsertIndex;
            //后续要改成按照C客户要求的算法来写

            for (int i = 0; i < WorkCount; i++)
            {
                foreach (var block in template_Blocks)
                {
                    inde++;
                    Recipe_Temp recipe_Temp = new Recipe_Temp
                    {
                        Blockuid = block.BlockUid,
                        BlockPN = block.BlockPn,
                        BlockAmount = block.BlockAmount,
                        RecipeIndex = inde,
                        RecipeType = RecipeTypeEnum.正常,
                        LayerNum = block.LayerNum,
                        IsEnable = true,
                        AttributionLine = block.AttributionLine,
                        PreallocationLine = null,
                        IssueLine = null,
                        PreallocationTime = null,
                        IssueTime = null,
                    };
                    RecipeTempss.Add(recipe_Temp);
                }
            }
        }

        /// <summary>
        /// 保存配方
        /// </summary>
        private void ExcuteRecipeSaveCommand()
        {
            if (RecipeTempss == null || RecipeTempss.Count <= 0)
                return;
            var result = MessageBox.Show("确认保存配方", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {            
                //非正常下发的要更新后面的序号
                if (RecipeTypes > 1)
                {
                    int recipecount = RecipeTempss.Count;
                    DbContext.GetInstance().Updateable<Recipe_Temp>().SetColumns(x => x.RecipeIndex == x.RecipeIndex + recipecount).Where(r => r.IsEnable == true && r.RecipeIndex > InsertIndex).ExecuteCommand();
                }
                DbContext.GetInstance().Insertable<Recipe_Temp>(RecipeTempss).ExecuteCommand();
                OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Delet, $"新增生产配方{RecipeTempss[0].BlockPN}，数量{WorkCount}");
            }
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }
    }
}
