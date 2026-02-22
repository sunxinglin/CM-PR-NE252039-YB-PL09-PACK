using MPAssmebleRecipe.Logger.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RogerTech.AuthService.Models;
using RogerTech.BussnessCore;
using RogerTech.Common;
using RogerTech.Common.AuthService.Model;
using RogerTech.Common.AuthService.Services;
using RogerTech.Common.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;

namespace MPAssmebleRecipe.Apps.ViewModels.Recipe
{
    public class CellRecipeViewModel : BindableBase, IRegionMemberLifetime
    {
        private readonly ILoggerHelper _logger;
        private readonly IDialogService _dialogService;      // 对话框服务
        public bool KeepAlive { get; set; } = true;

        private string StationName = "A";
        private string CurrentTable = "production_recipea";

        private ObservableCollection<Production_Recipe> recipeItems;
        public ObservableCollection<Production_Recipe> RecipeItems
        {
            get { return recipeItems; }
            set { SetProperty(ref recipeItems, value); }
        }
        
        public DelegateCommand RecipeJumpStepCommand { get; set; }
        public DelegateCommand RecipeClearCommand { get; set; }
        public DelegateCommand RecipeReplaceCommand { get; set; }
        public DelegateCommand RecipeRequestCommand { get; set; }
        public DelegateCommand RecipeBindingCommand { get; set; }

        public CellRecipeViewModel(ILoggerHelper logger, IDialogService dialogService)
        {
            _logger = logger;
            _dialogService = dialogService;
            StationName = ConfigurationManager.AppSettings["StationName"];
            CurrentTable = "production_recipe" + StationName.ToLower();
            RecipeItems = new ObservableCollection<Production_Recipe>();
            RecipeJumpStepCommand = new DelegateCommand(ExcuteRecipeJumpStepCommand);
            RecipeClearCommand = new DelegateCommand(ExcuteRecipeClearCommand);
            RecipeReplaceCommand = new DelegateCommand(ExcuteRecipeReplaceCommand);
            RecipeRequestCommand = new DelegateCommand(ExcuteRecipeRequestCommand);
            RecipeBindingCommand = new DelegateCommand(ExcuteRecipeBindingCommand);
            bool exists = DbContext.GetInstance().DbMaintenance.IsAnyTable(CurrentTable, false);
            if (!exists)
            {
                DbContext.GetInstance().DbMaintenance.CreateDatabase();
                DbContext.GetInstance().MappingTables.Add("Production_Recipe", CurrentTable);
                DbContext.GetInstance().CodeFirst.SetStringDefaultLength(200).InitTables<Production_Recipe>();
                DbContext.GetInstance().MappingTables.Clear();
            }
            DbContext.GetInstance().DbMaintenance.CreateDatabase();
            DbContext.GetInstance().CodeFirst.InitTables<Template_Block>();
            DbContext.GetInstance().CodeFirst.InitTables<Template_Module>();
            LoadRecipe();
            DbContext.ProgressInfoChange += new Action(() =>
            {
                LoadRecipe();
            });
        }

        private void LoadRecipe()
        {
            //Task.Run(new Action(() =>
            //{
            try
            {

                List<Production_Recipe> productions = DbContext.GetInstance().Queryable<Production_Recipe>().AS(CurrentTable)
                                                    //.Where(r => r.IsBinding == false || r.Issued == false)
                                                    .OrderBy(r => r.RecipeIndex).ToList();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    RecipeItems = new ObservableCollection<Production_Recipe>(productions);
                    RaisePropertyChanged(nameof(RecipeItems));
                });
            }
            catch (Exception)
            {
                RecipeItems = new ObservableCollection<Production_Recipe>();
            }
           // }));
        }


        /// <summary>
        /// 跳回上一步
        /// </summary>
        private void ExcuteRecipeJumpStepCommand()
        {
            if (string.IsNullOrEmpty(StationName)) return;
            var result = MessageBox.Show("确认是否跳回上一步配方", "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Production_Recipe productionrecipe = DbContext.GetInstance().Queryable<Production_Recipe>().AS(CurrentTable).Where(r => r.Issued == true).OrderBy(r => r.ID, OrderByType.Desc).First();
                if (productionrecipe == null)
                {
                    MessageBox.Show("未查到配方详细信息");
                    return;
                }
                productionrecipe.Issued = false;
                productionrecipe.IsBinding = false;
                productionrecipe.PalletIndex = null;
                productionrecipe.CellSn = null;
                int rowcount = DbContext.GetInstance().Updateable<Production_Recipe>(productionrecipe).AS(CurrentTable).ExecuteCommand();
                OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Delet, $"用户进行配方详细信息跳回上一步,数量:{rowcount}");
            }
            LoadRecipe();
        }
        /// <summary>
        /// 清空配方
        /// </summary>
        private void ExcuteRecipeClearCommand()
        {
            if (string.IsNullOrEmpty(StationName)) return;
            var result = MessageBox.Show("确认是否清空配方", "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                int rowcount = DbContext.GetInstance().Updateable<Production_Recipe>().AS(CurrentTable).SetColumns(r => new Production_Recipe { Issued = true, IsBinding = true }).Where(p => p.IsBinding == false || p.Issued == false).ExecuteCommand();
                OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Delet, $"用户进行配方详细信息清空,数量:{rowcount}");
            }
        }
        /// <summary>
        /// 电芯码替换
        /// </summary>
        private void ExcuteRecipeReplaceCommand()
        {
            if (string.IsNullOrEmpty(StationName)) return;
            try
            {
                IDialogParameters dialogParameters = new DialogParameters()
                {
                    { "StationName", StationName}
                };
                _dialogService.ShowDialog("CellRecipeReplaceView", dialogParameters, result =>
                {
                    if (result.Result == ButtonResult.OK)
                    {
                        LoadRecipe();
                    }
                });
            }
            catch (Exception ex)
            {
              //  LogHelper.Errors("电芯码替换失败");
                MessageBox.Show("电芯码替换失败：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 测试
        /// </summary>
        private void ExcuteRecipeRequestCommand()
        {
            if (string.IsNullOrEmpty(StationName)) return;
            //先查询Production_Recipe表，Issued未下发的第一笔信息
            Production_Recipe productionrecipe = DbContext.GetInstance().Queryable<Production_Recipe>().AS(CurrentTable).Where(r => r.Issued == false).OrderBy(r => r.ID).First();
            if (productionrecipe == null)
            {
                //没有可下发的配方，去Recipe_Temp表抢配方；
                //1、先获取第一条有效的配方
                Recipe_Temp recipe_Temp = DbContext.GetInstance().Queryable<Recipe_Temp>().Where(r => r.PreallocationLine == StationName && r.IsEnable == true).First();
                if (recipe_Temp == null)
                {
                    //2、抢配方顺序：开拉配方 => 正常配方 => 收拉配方，同步更新预分配拉线和预分配时间
                    string sqltext = $"update  public.recipe_temp rc1 set preallocationline = '{StationName}',preallocationtime = LOCALTIMESTAMP " +
                                     $"where rc1.ID = (select rc2.ID from (" +
                                     $"select rc3.ID from public.recipe_temp rc3 where rc3.preallocationline is null or rc3.preallocationline = '' " +
                                     $"order by rc3.recipetype,rc3.recipeindex asc) rc2 LIMIT 1)";
                    DbContext.GetInstance().Ado.ExecuteCommand(sqltext);
                    //3、获取未下发的配方信息
                    recipe_Temp = DbContext.GetInstance().Queryable<Recipe_Temp>().Where(r => r.PreallocationLine == StationName && r.IsEnable == true).First();
                }
                if(recipe_Temp == null)
                {
                    MessageBox.Show("未抢到配方信息");
                    return;
                }
                //4、找到Block下配方信息
                List<TemplateDto> templateDtos = DbContext.GetInstance().Queryable<Template_Block, Template_Module, Template_Cell>((t1, t2, t3) => t1.Id == t2.BlockId && t2.Id == t3.ModuleId)
                    .Where((t1, t2) => t1.BlockUid == recipe_Temp.Blockuid)
                    .Select<TemplateDto>((t1, t2, t3) => new TemplateDto()
                    {
                        BlockPn = t1.BlockPn,
                        BlockCount = t1.BlockAmount,
                        BlockIndex = t1.BlockIndex,
                        LayerNum = t1.LayerNum,
                        LineNum = t1.LineNum,
                        LimitPlate = t1.LimitPlate,
                        SplitType = t1.SplitType,
                        ModuleColums = t1.ModuleAmount,
                        ModuleIndex = t2.ModuleIndex,
                        ModuleFaceTape = t2.Side1Tape,
                        ModuleSideTape = t2.Side2Tape,
                        ModuleType = t2.ModuleType,
                        ModulePolarity = t2.ModulePolarity,
                        ModuleRotate = t2.ModuleRotate,
                        WaterCoolType = t2.WaterCoolType,
                        WaterTubeType = t2.WaterTubeType,
                        SulationCover = t2.SulationCover,
                        CellCount = t2.CellAmount,
                        CellIndex = t3.CellIndex,
                        CellPolarity = t3.Polarity,
                        CellSideTape1 = t3.Side1Tape,
                        CellSideTape2 = t3.Side2Tape
                    }).ToList();
                if (templateDtos == null)
                {
                    MessageBox.Show("未查到配方详细信息");
                    return;
                }
                templateDtos = templateDtos.OrderBy(p => p.ModuleIndex).ThenBy(x => x.CellIndex).ToList();
                //5、全部转换成配方信息
                List<Production_Recipe> production_Recipes = new List<Production_Recipe>();
                int blockcount = templateDtos[0].BlockCount;
                if (blockcount == 0)
                {
                    MessageBox.Show("请维护Block表中模组数量信息");
                    return;
                }
                int recipeindex = 0;
                int recipeindexmax = blockcount * templateDtos.Count;
                int cellindexmin = templateDtos.Min(r => r.CellIndex);
                int cellindexmax = templateDtos.Max(r => r.CellIndex);
                for (int i = 0; i < blockcount; i++)
                {
                    //申请模组码
                    string blocksn = "001MZ0" + DateTime.Now.ToString("yyyyMMddHHmmssffff");
                    foreach (var item in templateDtos)
                    {
                        recipeindex++;
                        Production_Recipe production_Recipe = new Production_Recipe()
                        {
                            Issued = false,
                            IsBinding = false,
                            RecipeId = recipe_Temp.ID,
                            RecipeIndex = recipeindex,
                            BlockPn = item.BlockPn,
                            BlcokSn = blocksn,
                            CellSn = null,
                            PalletIndex = null,
                            CellPolarity = item.CellPolarity ? 1 : -1,
                            Side1Tape = (int)item.CellSideTape1,
                            Side2Tape = (int)item.CellSideTape2,
                            BlockCount = item.BlockCount,
                            BlockIndex = item.BlockIndex,
                            PillarCount = item.ModuleColums,
                            PillarIndex = item.ModuleIndex,
                            PillarRotate = item.ModuleRotate,
                            PillarCellCount = item.CellCount,
                            SplitType = item.SplitType,
                            RecipeRemaining = recipeindexmax - recipeindex,
                            LayerNum = item.LayerNum,
                            NextRecipe = "",
                            ProductionEnd = false,
                            CreateTime = DateTime.Now,
                        };
                        if (recipeindex == 1)
                        {
                            production_Recipe.RecipeStrat = true;
                            production_Recipe.RecipeEnd = false;
                        }
                        else if (recipeindex == recipeindexmax)
                        {
                            production_Recipe.RecipeStrat = false;
                            production_Recipe.RecipeEnd = true;
                        }
                        else
                        {
                            production_Recipe.RecipeStrat = false;
                            production_Recipe.RecipeEnd = false;
                        }

                        if (item.CellIndex == cellindexmin)
                        {
                            production_Recipe.PillarStart = true;
                            production_Recipe.PillarEnd = false;
                        }
                        else if (item.CellIndex == cellindexmax)
                        {
                            production_Recipe.PillarStart = false;
                            production_Recipe.PillarEnd = true;
                        }
                        else
                        {
                            production_Recipe.PillarStart = false;
                            production_Recipe.PillarEnd = false;
                        }
                        production_Recipes.Add(production_Recipe);
                    }
                }
                //6、写入Production_Recipe表
                int insertrowcount = DbContext.GetInstance().Insertable<Production_Recipe>(production_Recipes).AS(CurrentTable).ExecuteCommand();
                if (insertrowcount != production_Recipes.Count)
                {
                    MessageBox.Show($"配方信息存储失败：总数量{production_Recipes.Count}，成功{insertrowcount}");
                    return;
                }
                //7、更新下发拉线、下发时间、是否生效为False
                recipe_Temp.IssueLine = StationName;
                recipe_Temp.IssueTime = DateTime.Now;
                recipe_Temp.IsEnable = false;
                for (int i = 0; i < 5; i++)
                {
                    int upreciperowcount = DbContext.GetInstance().Updateable<Recipe_Temp>(recipe_Temp).ExecuteCommand();
                    if (upreciperowcount > 0)
                    {
                        break;
                    }
                    Thread.Sleep(100);
                }
                //8、再次查询Production_Recipe表第一笔
                productionrecipe = DbContext.GetInstance().Queryable<Production_Recipe>().AS(CurrentTable).Where(r => r.Issued == false).OrderBy(r => r.ID).First();
                if (productionrecipe == null)
                {
                    MessageBox.Show("未查到配方详细信息");
                    return;
                }
            }
            //Issued更新到Production_Recipe表
            productionrecipe.Issued = true;
            DbContext.GetInstance().Updateable<Production_Recipe>(productionrecipe).AS(CurrentTable).ExecuteCommand();
            //下发信息给PLC

            LoadRecipe();
        }
        private void ExcuteRecipeBindingCommand()
        {
            Production_Recipe productionrecipe = DbContext.GetInstance().Queryable<Production_Recipe>().AS(CurrentTable).Where(r => r.IsBinding == false).OrderBy(r => r.RecipeIndex).First();
            if (productionrecipe == null)
            {
                MessageBox.Show("未查到配方详细信息");
                return;
            }
            productionrecipe.IsBinding = true;
            productionrecipe.PalletIndex = new Random().Next(1, 50);
            productionrecipe.CellSn = "001CL0" + DateTime.Now.ToString("yyyyMMddHHmmssffff");
            DbContext.GetInstance().Updateable<Production_Recipe>(productionrecipe).AS(CurrentTable).ExecuteCommand();
            LoadRecipe();
        }
    }
}
