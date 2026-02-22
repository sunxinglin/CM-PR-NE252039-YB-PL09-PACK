using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using RogerTech.Common.Models;
using MPAssmebleRecipe.Logger.Interfaces;
using MPAssmebleRecipe.Apps.Common;
using Prism.Services.Dialogs;
using MPAssmebleRecipe.Apps.ViewModels.ModuleManage;
using System.Linq;
using System.Collections.Generic;
using RogerTech.Common.AuthService.Services;
using System.Runtime.Remoting.Contexts;
using System.Reflection;
using RogerTech.Common;

namespace MPAssmebleRecipe.Apps.ViewModels.PackManage
{
    /// <summary>
    /// Pack管理视图模型
    /// </summary>
    public class UcPackManageViewModel : ViewModelBase
    {
        #region Fields
        private readonly ILoggerHelper _logger;              // 日志服务
        private readonly IDialogService _dialogService;      // 对话框服务

        private Template_Pack _selectedPack;                 // 选中的Pack
        private Template_Block _selectedBlock;               // 选中的Block
        private Template_Module _selectedModule;             // 选中的Unit
        private Template_Cell _selectedCell;                 // 选中的Cell

        private ObservableCollection<Template_Pack> _packs;          // Pack列表
        private ObservableCollection<Template_Block> _blocksItems;   // Block列表
        private ObservableCollection<Template_Module> _modulesItems; // Unit列表
        private ObservableCollection<Template_Cell> _cellItems;      // Cell列表
        #endregion

        #region Commands 命令

        /// <summary>
        /// 添加Pack命令
        /// </summary>
        public DelegateCommand AddPackCommand { get; private set; }
        /// <summary>
        /// 删除Pack命令
        /// </summary>
        public DelegateCommand DeletePackCommand { get; private set; }

        /// <summary>
        /// 添加Block命令
        /// </summary>
        public DelegateCommand AddBlockCommand { get; private set; }
        /// <summary>
        /// 删除Block命令
        /// </summary>
        public DelegateCommand DelBlockCommand { get; private set; }
        /// <summary>
        /// 维护Block命令
        /// </summary>
        public DelegateCommand EditBlockCommand { get; private set; }

        /// <summary>
        /// 维护模组命令
        /// </summary>
        public DelegateCommand EditModuleCommand { get; private set; }
        /// <summary>
        /// 维护电芯命令
        /// </summary>
        public DelegateCommand EditCellCommand { get; private set; }
        #endregion

        #region Constructor 构造函数
        /// <summary>
        /// 构造函数，初始化服务、命令和数据集合
        /// </summary>
        /// <param name="logger">日志服务</param>
        /// <param name="packService">Pack管理服务</param>
        /// <param name="dialogService">对话框服务</param>
        public UcPackManageViewModel(ILoggerHelper logger, IDialogService dialogService) : base(logger)
        {
            _logger = logger;
            _dialogService = dialogService;
            ModuleDisplay = new UcModuleDisplayViewModel();
            // 初始化命令
            InitializeCommands();
            // 初始化集合
            //InitializeCollections();
            // 加载Pack列表
            LoadPacks();
        }
        #endregion

        #region 属性
        /// <summary>
        /// Pack列表
        /// </summary>
        public ObservableCollection<Template_Pack> Packs
        {
            get => _packs;
            set
            {
                if (SetProperty(ref _packs, value))
                {
                    // 更新命令可执行状态
                    DeletePackCommand.RaiseCanExecuteChanged();
                    AddBlockCommand.RaiseCanExecuteChanged();
                    if (SelectedPack == null) { return; }
                }
            }

        }
        /// <summary>
        /// 选中的Pack
        /// </summary>
        public Template_Pack SelectedPack
        {
            get => _selectedPack;
            set
            {
                if (SetProperty(ref _selectedPack, value))
                {
                    if (SelectedPack == null) { return; }
                    DeletePackCommand.RaiseCanExecuteChanged();
                    LoadBlocks();
                }
            }
        }

        /// <summary>
        /// Block列表集合
        /// </summary>
        public ObservableCollection<Template_Block> BlocksItems
        {
            get => _blocksItems;
            set => SetProperty(ref _blocksItems, value);
        }
        /// <summary>
        /// 选中的Block，变更时触发Unit列表加载
        /// </summary>
        public Template_Block SelectedBlock
        {
            get => _selectedBlock;
            set
            {
                if (SetProperty(ref _selectedBlock, value))
                {
                    LoadModules();  // 加载对应的Unit列表

                    // 清空下级选择
                    SelectedModule = null;
                    CellItems?.Clear();
                }
            }
        }

        /// <summary>
        /// Unit列表集合
        /// </summary>
        public ObservableCollection<Template_Module> ModulesItems
        {
            get => _modulesItems;
            set => SetProperty(ref _modulesItems, value);
        }
        /// <summary>
        /// 选中的Unit，变更时触发Cell列表加载
        /// </summary>
        public Template_Module SelectedModule
        {
            get => _selectedModule;
            set
            {
                if (SetProperty(ref _selectedModule, value))
                {
                    EditCellCommand.RaiseCanExecuteChanged();
                    LoadCells();
                }
            }
        }

        /// <summary>
        /// Cell列表集合
        /// </summary>
        public ObservableCollection<Template_Cell> CellItems
        {
            get => _cellItems;
            set => SetProperty(ref _cellItems, value);
        }

        /// <summary>
        /// 选中的Cell
        /// </summary>
        public Template_Cell SelectedCell
        {
            get => _selectedCell;
            set => SetProperty(ref _selectedCell, value);
        }

        #endregion


        #region Private Methods
        /// <summary>
        /// 初始化所有命令
        /// </summary>
        private void InitializeCommands()
        {
            AddPackCommand = new DelegateCommand(ExecuteAddPack);
            DeletePackCommand = new DelegateCommand(ExecuteDeletePack, CanExecuteDeletePack);
            AddBlockCommand = new DelegateCommand(ExecuteAddBlock, CanExecuteAddBlock);
            DelBlockCommand = new DelegateCommand(ExecuteDelBlock, CanExecuteDelBlock);
            EditBlockCommand = new DelegateCommand(ExecuteEditBlock, CanExecuteEditBlock);
            EditModuleCommand = new DelegateCommand(ExecuteEditModule, CanExecuteEditModule);
            EditCellCommand = new DelegateCommand(ExecuteEditCell, CanExecuteEditCell);
        }
        /// <summary>
        /// 初始化数据集合
        /// </summary>
        private void InitializeCollections()
        {
            BlocksItems = new ObservableCollection<Template_Block>();
            ModulesItems = new ObservableCollection<Template_Module>();
            CellItems = new ObservableCollection<Template_Cell>();
        }

        /// <summary>
        /// 执行添加Pack命令
        /// </summary>
        private void ExecuteAddPack()
        {
            try
            {
                IDialogParameters dialogParameters = new DialogParameters();
                _dialogService.ShowDialog("UcPackAddView", dialogParameters, result =>
                {
                    if (result.Result == ButtonResult.OK)
                    {
                        var packTemplate = result.Parameters.GetValues<Template_Pack>("Pack").First();
                        //   SelectedPack.BlockGroups.Add(blockGroup);
                        DbContext.GetInstance().Storageable<Template_Pack>(packTemplate).ExecuteCommand();
                        OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Add, $"用户新增Pack,PackPn[{packTemplate.PackPn}]");
                        LoadPacks(); // 重新加载Pack列表
                    }
                });
            }
            catch (Exception ex)
            {
               // LogHelper.Errors("打开Pack添加对话框失败");
                MessageBox.Show($"操作失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 判断是否可以执行删除Pack命令
        /// </summary>
        private bool CanExecuteDeletePack()
        {
            return SelectedPack != null;
        }
        /// <summary>
        /// 执行删除Pack命令
        /// </summary>
        private void ExecuteDeletePack()
        {
            try
            {
                if (SelectedPack == null) return;
                if (MessageBox.Show("确定要删除选中的Pack吗？", "确认",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;

                DbContext.GetInstance().DeleteNav<Template_Pack>(x => x.Id == SelectedPack.Id)
                   .Include(x => x.BlockItems)
                   .ThenInclude(m => m.ModuleItems)
                   .ThenInclude(c => c.CellItems)
                   .ExecuteCommand();
                DbContext.GetInstance().Deleteable<Template_Pack>().Where(x => x.Id == SelectedPack.Id).ExecuteCommand();
                OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Delet, $"用户删除Pack,PackPn[{SelectedPack.PackPn}]");
                LoadPacks();
                LoadBlocks();
            }
            catch (Exception ex)
            {
               // LogHelper.Errors("删除Pack失败");
                MessageBox.Show("删除Pack失败：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 检查是否可以执行添加Block操作
        /// </summary>
        private bool CanExecuteAddBlock()
        {
            return SelectedPack != null;
        }
        /// <summary>
        /// 执行添加Block操作
        /// </summary>
        private void ExecuteAddBlock()
        {
            try
            {
                if (SelectedPack == null)
                {
                    MessageBox.Show("请先选择Pack！");
                    return;
                }

                IDialogParameters dialogParameters = new DialogParameters()
                {
                    { "PackId",SelectedPack.Id},
                    { "PackPn",SelectedPack.PackPn}
                };

                _dialogService.ShowDialog("UcBlockAddView", dialogParameters, result =>
                {
                    if (result.Result == ButtonResult.OK)
                    {
                        var blocks = result.Parameters.GetValues<Template_Block>("Blocks").First();
                        DbContext.GetInstance().InsertNav<Template_Block>(blocks)
                                              .Include(m => m.ModuleItems)
                                              .ThenInclude(c => c.CellItems)
                                              .ExecuteCommand();
                        OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Add, $"用户新增Block,BlockPn[{blocks.BlockPn}]");
                        LoadBlocks();
                    }
                });
            }
            catch (Exception ex)
            {
              //  LogHelper.Errors("添加Block失败");
                MessageBox.Show("添加Block失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 判断是否可以执行删除Block命令
        /// </summary>
        private bool CanExecuteDelBlock()
        {
            return SelectedBlock != null && BlocksItems != null && BlocksItems.Count > 0;
        }
        /// <summary>
        /// 执行删除Block命令
        /// </summary>
        private void ExecuteDelBlock()
        {
            if (SelectedBlock != null)
            {
                if (MessageBox.Show("确定要删除选中的Block吗？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;

                DbContext.GetInstance().DeleteNav<Template_Block>(x => x.Id == SelectedBlock.Id)
                    .Include(m => m.ModuleItems)
                    .ThenInclude(c => c.CellItems)
                    .ExecuteCommand();
                //重新排序
                var blocks = DbContext.GetInstance().Queryable<Template_Block>().Where(x => x.PackId == SelectedPack.Id).OrderBy(r => r.BlockIndex).ToList();
                int i = 1;
                foreach (var block in blocks)
                {
                    block.BlockIndex = i;
                    i++;
                }
                DbContext.GetInstance().Updateable<Template_Block>(blocks).UpdateColumns(x => x.BlockIndex).ExecuteCommand();
                OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Add, $"用户删除Block,BlockPn[{SelectedBlock.BlockPn}]");
                LoadBlocks();
            }
        }

        /// <summary>
        /// 检查是否可以执行维护Block操作
        /// </summary>
        private bool CanExecuteEditBlock()
        {
            return SelectedPack != null && BlocksItems != null && BlocksItems.Count > 0;
        }
        /// <summary>
        /// 执行维护Block操作
        /// </summary>
        private void ExecuteEditBlock()
        {
            try
            {
                if (BlocksItems == null || BlocksItems.Count <= 0)
                {
                    MessageBox.Show("请先选择要维护的Block信息！");
                    return;
                }
                IDialogParameters dialogParameters = new DialogParameters()
                {
                    { "PackId", SelectedPack.Id}
                };

                _dialogService.ShowDialog("UcBlockEditView", dialogParameters, result =>
                {
                    if (result.Result == ButtonResult.OK)
                    {
                        LoadBlocks();
                    }
                });
            }
            catch (Exception ex)
            {
              //  LogHelper.Errors("维护Block失败");
                MessageBox.Show("维护Block失败：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 检查是否可以执行维护Unit操作
        /// </summary>
        private bool CanExecuteEditModule()
        {
            return SelectedBlock != null && ModulesItems != null && ModulesItems.Count > 0;
        }
        /// <summary>
        /// 执行维护Unit操作
        /// </summary>
        private void ExecuteEditModule()
        {
            try
            {
                if (ModulesItems == null || ModulesItems.Count <= 0)
                {
                    MessageBox.Show("请先选择要维护的Unit信息！");
                    return;
                }
                IDialogParameters dialogParameters = new DialogParameters()
                {
                    { "BlockUid", SelectedBlock.BlockUid}
                };

                _dialogService.ShowDialog("UcModuleEditView", dialogParameters, result =>
                {
                    if (result.Result == ButtonResult.OK)
                    {
                        LoadModules();
                        // 清空下级选择
                        SelectedModule = null;
                        CellItems?.Clear();
                    }
                });
            }
            catch (Exception ex)
            {
              //  LogHelper.Errors("维护Block失败");
                MessageBox.Show("维护Block失败：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 检查是否可以执行维护Cell操作
        /// </summary>
        private bool CanExecuteEditCell()
        {
            return SelectedModule != null && CellItems != null && CellItems.Count > 0;
        }
        /// <summary>
        /// 执行维护Cell操作
        /// </summary>
        private void ExecuteEditCell()
        {
            try
            {
                if (CellItems == null || CellItems.Count <= 0)
                {
                    MessageBox.Show("请先选择要维护的Cell信息！");
                    return;
                }
                IDialogParameters dialogParameters = new DialogParameters()
                {
                    { "ModuleId", SelectedModule.Id}
                };

                _dialogService.ShowDialog("UcCellEditView", dialogParameters, result =>
                {
                    if (result.Result == ButtonResult.OK)
                    {
                        LoadModules();
                        // 清空下级选择
                        SelectedModule = null;
                        CellItems?.Clear();
                    }
                });
            }
            catch (Exception ex)
            {
              //  LogHelper.Errors("维护Cell失败");
                MessageBox.Show("维护Cell失败：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// 加载Pack列表
        /// </summary>
        private void LoadPacks()
        {
            try
            {
                InitializeCollections();
                var packs = DbContext.GetInstance().Queryable<Template_Pack>().ToList();
                Packs = new ObservableCollection<Template_Pack>(packs);
            }
            catch (Exception ex)
            {
              //  LogHelper.Errors("加载Pack列表失败");
                MessageBox.Show("加载Pack列表失败：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                AddBlockCommand.RaiseCanExecuteChanged();
                DelBlockCommand.RaiseCanExecuteChanged();
                EditBlockCommand.RaiseCanExecuteChanged();
                EditModuleCommand.RaiseCanExecuteChanged();
                EditCellCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// 加载Block列表
        /// </summary>
        private void LoadBlocks()
        {
            try
            {
                if (SelectedPack != null)
                {
                    List<Template_Block> blocks = DbContext.GetInstance().Queryable<Template_Block>().Where(x => x.PackId == SelectedPack.Id).ToList();
                    if(blocks != null)
                        blocks = blocks.OrderBy(p => p.BlockSequence).ThenBy(x => x.BlockIndex).ToList();
                    BlocksItems = new ObservableCollection<Template_Block>(blocks);
                    SelectedPack.BlockItems = blocks;
                    Logger.Info($"成功加载Pack({SelectedPack.PackPn})的Block列表");
                }
                else
                {
                    BlocksItems.Clear();
                    Logger.Info("清空Block列表");
                }
            }
            catch (Exception ex)
            {
               // LogHelper.Errors("加载Block列表失败");
                MessageBox.Show("加载Block列表失败：" + ex.Message);
            }
            finally
            {
                AddBlockCommand.RaiseCanExecuteChanged();
                DelBlockCommand.RaiseCanExecuteChanged();
                EditBlockCommand.RaiseCanExecuteChanged();
                EditModuleCommand.RaiseCanExecuteChanged();
                EditCellCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// 加载Unit列表
        /// </summary>
        private void LoadModules()
        {
            try
            {
                if (SelectedBlock != null)
                {
                    var modules = DbContext.GetInstance().Queryable<Template_Module>().Where(p => p.ModuleUid == SelectedBlock.BlockUid).OrderBy(r => r.ModuleIndex).ToList();
                    ModulesItems = new ObservableCollection<Template_Module>(modules);
                    SelectedBlock.ModuleItems = modules;
                    Logger.Info($"成功加载Block({SelectedBlock.BlockPn})的Unit列表");
                }
                else
                {
                    ModulesItems.Clear();
                    Logger.Info("清空Unit列表");
                }
            }
            catch (Exception ex)
            {
              //  LogHelper.Errors("加载Unit列表失败");
                MessageBox.Show("加载Unit列表失败：" + ex.Message);
            }
            finally
            {
                DelBlockCommand.RaiseCanExecuteChanged();
                EditModuleCommand.RaiseCanExecuteChanged();
                EditCellCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// 加载Cell列表
        /// </summary>
        private void LoadCells()
        {
            try
            {
                if (SelectedModule != null)
                {
                    var celles = DbContext.GetInstance().Queryable<Template_Cell>().Where(p => p.ModuleId == SelectedModule.Id).OrderBy(r=>r.CellIndex).ToList();
                    CellItems = new ObservableCollection<Template_Cell>(celles);
                    SelectedModule.CellItems = celles;
                    Logger.Info($"成功加载Unit({SelectedModule.BlockPn})的Cell列表");
                }
                else
                {
                    CellItems.Clear();
                    Logger.Info("清空Cell列表");
                }
                // 更新模组显示
                ModuleDisplay.UpdateDisplay(CellItems.ToList());
            }
            catch (Exception ex)
            {
              //  LogHelper.Errors("加载Cell列表失败");
                MessageBox.Show("加载Cell列表失败：" + ex.Message);
            }
        }
        #endregion


        private UcModuleDisplayViewModel _moduleDisplay;
        /// <summary>
        /// 模组显示视图模型
        /// </summary>
        public UcModuleDisplayViewModel ModuleDisplay
        {
            get => _moduleDisplay;
            set => SetProperty(ref _moduleDisplay, value);
        }
    }
}