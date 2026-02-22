using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using MPAssmebleRecipe.Logger.Interfaces;
using RogerTech.Common.Models;
using MPAssmebleRecipe.Apps.Common;
using System.Windows;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using RogerTech.Common;

namespace MPAssmebleRecipe.Apps.ViewModels.ModuleManage
{
    /// <summary>
    /// 模组添加视图模型
    /// </summary>
    public class UcModuleAddViewModel : ViewModelBase, IDialogAware
    {
  
        private readonly UcModuleDisplayViewModel _moduleDisplayViewModel;

        /// <summary>
        /// 当前模组ID，用于保存后的电芯操作
        /// </summary>
        private int _currentModuleId;

        #region IDialogAware Implementation
        public event Action<IDialogResult> RequestClose;
        public string Title => "添加模组";

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            // 清理资源
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            // 获取传入的参数
            if (parameters.ContainsKey("BlockId"))
            {
                BlockId = parameters.GetValue<int>("BlockId");
            }
            if (parameters.ContainsKey("BlockPn"))
            {
                ModulePn = parameters.GetValue<string>("BlockPn");
            }
            //if (parameters.ContainsKey("BlockGroupId"))
            //{
            //    BlockGroupId = parameters.GetValue<int>("BlockGroupId");
            //}
            if (parameters.ContainsKey("BlockName"))
            {
                ModuleName = parameters.GetValue<string>("BlockName");
            }
            if (parameters.ContainsKey("ModuleColumns"))
            {
                _moduleColumns = parameters.GetValue<ModuleColumns>("ModuleColumns");
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// 所属Block的ID
        /// </summary>
        private int BlockId { get; set; }

        /// <summary>
        /// Block单双排配置
        /// </summary>
        private ModuleColumns _moduleColumns { get; set; }

        /// <summary>
        /// 模组名称
        /// </summary>
        private string _moduleName;
        public string ModuleName
        {
            get => _moduleName;
            set => SetProperty(ref _moduleName, value);
        }

        /// <summary>
        /// 模组料号
        /// </summary>
        private string _modulePn;
        public string ModulePn
        {
            get => _modulePn;
            set => SetProperty(ref _modulePn, value);
        }

        /// <summary>
        /// 电芯数量
        /// </summary>
        private int _cellCount = 0;
        public int CellCount
        {
            get => _cellCount;
            set => SetProperty(ref _cellCount, value);
        }

        /// <summary>
        /// 所属BlockGroup的ID
        /// </summary>
        //private int _blockGroupId;
        //public int BlockGroupId
        //{
        //    get => _blockGroupId;
        //    set => SetProperty(ref _blockGroupId, value);
        //}

        /// <summary>
        /// 是否可以保存模组
        /// </summary>
        private bool _canSaveModule = true;
        public bool CanSaveModule
        {
            get => _canSaveModule;
            set
            {
                if (SetProperty(ref _canSaveModule, value))
                {
                    // 通知命令状态已更改
                    SaveModuleCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #region Cell Properties

        /// <summary>
        /// 电芯模板集合
        /// </summary>
        public ObservableCollection<Template_Cell> CellTemplates { get; } = new ObservableCollection<Template_Cell>();

        /// <summary>
        /// 当前选中的电芯模板
        /// </summary>
        private Template_Cell _selectedCellTemplate;
        public Template_Cell SelectedCellTemplate
        {
            get => _selectedCellTemplate;
            set
            {
                if (SetProperty(ref _selectedCellTemplate, value) && value != null)
                {
                    // 更新UI显示
                    IsPositive = value.Polarity;
                    IsNegative = !value.Polarity;
                    FrontTape = (CellSideTapeType)value.Side1Tape;
                    BackTape = value.Side2Tape;
                    Logger.Info($"选择电芯模板：序号{value.CellIndex}");
                }
            }
        }

        /// <summary>
        /// 是否为正极
        /// </summary>
        private bool _isPositive = true;
        public bool IsPositive
        {
            get => _isPositive;
            set
            {
                if (SetProperty(ref _isPositive, value) && value)
                {
                    IsNegative = !value;
                }
            }
        }

        /// <summary>
        /// 是否为负极
        /// </summary>
        private bool _isNegative;
        public bool IsNegative
        {
            get => _isNegative;
            set
            {
                if (SetProperty(ref _isNegative, value) && value)
                {
                    IsPositive = !value;
                }
            }
        }

        /// <summary>
        /// 前端胶条
        /// </summary>
        private CellSideTapeType _frontTape = CellSideTapeType.不贴;
        public CellSideTapeType FrontTape
        {
            get => _frontTape;
            set
            {
                if (SetProperty(ref _frontTape, value) && SelectedCellTemplate != null)
                {
                    SelectedCellTemplate.Side1Tape = (CellFaceTapeType)value;
                }
            }
        }

        /// <summary>
        /// 后端胶条
        /// </summary>
        private CellSideTapeType _backTape = CellSideTapeType.不贴;
        public CellSideTapeType BackTape
        {
            get => _backTape;
            set
            {
                if (SetProperty(ref _backTape, value) && SelectedCellTemplate != null)
                {
                     SelectedCellTemplate.Side2Tape = value;
                }
            }
        }

        private UcModuleDisplayViewModel _moduleDisplay;
        /// <summary>
        /// 模组显示视图模型
        /// </summary>
        public UcModuleDisplayViewModel ModuleDisplay
        {
            get => _moduleDisplay;
            set => SetProperty(ref _moduleDisplay, value);
        }

        #endregion

        #endregion

        #region Commands
        /// <summary>
        /// 保存模组命令
        /// </summary>
        public DelegateCommand SaveModuleCommand { get; }

        /// <summary>
        /// 取消命令
        /// </summary>
        public DelegateCommand CancelCommand { get; }

        /// <summary>
        /// 预览命令
        /// </summary>
        public DelegateCommand PreviewCommand { get; }

        /// <summary>
        /// 保存电芯命令
        /// </summary>
        public DelegateCommand SaveCellCommand { get; }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public UcModuleAddViewModel(
            ILoggerHelper logger,     
            UcModuleDisplayViewModel moduleDisplayViewModel) : base(logger)
        {
  
            _moduleDisplayViewModel = moduleDisplayViewModel;

            // 初始化命令
            SaveModuleCommand = new DelegateCommand(ExecuteSaveModule);
            CancelCommand = new DelegateCommand(ExecuteCancel);
            PreviewCommand = new DelegateCommand(ExecutePreview);

            ModuleDisplay = new UcModuleDisplayViewModel();
            SaveCellCommand = new DelegateCommand(ExecuteSaveCell);
        }

        /// <summary>
        /// 保存模组
        /// </summary>
        private void ExecuteSaveModule()
        {
            try
            {
                // 验证输入
                if (string.IsNullOrWhiteSpace(ModuleName) || string.IsNullOrWhiteSpace(ModulePn))
                {
                    MessageBox.Show("请填写完整的模组信息！");
                    return;
                }
                if (BlockId <= 0)
                {                
                    MessageBox.Show("无效的Block ID");
                    return;
                }
                // 验证电芯数量
                if (CellCount <= 0)
                {
                    MessageBox.Show("请输入有效的电芯数量！");
                    return;
                }

                // 验证排列
                var moduleCount = DbContext.GetInstance().Queryable<Template_Module>()
                  .Where(m => m.BlockId == BlockId)
                  .OrderBy(m => m.ModuleIndex).ToList();
                if (moduleCount.Count + 1 > (int)_moduleColumns)
                {
                    MessageBox.Show($"当前Block为{(int)_moduleColumns}排Block，请检查模组数量");
                    return;
                }
                ModuleAddDto moduleDto = new ModuleAddDto
                {
                    Name = ModuleName,
                    Pn = ModulePn,
                    CellCount = CellCount,
                    BlockId = BlockId,
                    //BlockGroupId = BlockGroupId,
                };
                int nextModuleIndex = 1;
                var existingModules = DbContext.GetInstance().Queryable<Template_Module>()
                .Where(m => m.BlockId == BlockId)
                .OrderBy(m => m.ModuleIndex).ToList();


                if (existingModules != null && existingModules.Any())
                {
                    // 如果有现有模组，获取最大序号并加1
                    nextModuleIndex = existingModules.Max(m => m.ModuleIndex) + 1;
                }             

                // 创建Module实体
                var module = new Template_Module
                {
                    BlockSequence = 1,
                    BlockPn = moduleDto.Pn,
                    BlockId = moduleDto.BlockId,
                    //BlockGroupId = moduleDto.BlockGroupId,
                    CellAmount = moduleDto.CellCount,
                    ModuleIndex = nextModuleIndex,  // 设置模组序号
                };


          
                int resultModuleId = -1;
                var result = DbContext.GetInstance().Ado.UseTran(() =>
                {
                    // 1. 添加Module
                    resultModuleId = DbContext.GetInstance().Insertable(module).ExecuteReturnIdentity();                
                    // 2. 创建Cells
                    var cells = new List<Template_Cell>();
                    for (int i = 0; i < module.CellAmount; i++)
                    {
                        cells.Add(new Template_Cell
                        {
                            ModuleId = resultModuleId,
                            CellIndex = i + 1,
                            Polarity = i % 2 == 0, // true为正极，false为负极，交替设置
                            BlockPn = module.BlockPn
                        });
                    }

                    // 3. 批量插入Cells
                    var cellResult = DbContext.GetInstance().Insertable(cells).ExecuteCommand() == module.CellAmount;
                    if (!cellResult)
                    {
                        throw new Exception("添加Cells失败");
                    }
                });              
            
                // 保存模组信息             

                if (result.IsSuccess)
                {
                    _currentModuleId = resultModuleId;
                    CanSaveModule = false; // 禁用保存模组按钮
                    Logger.Info($"成功添加模组：{ModuleName}");

                    // 刷新电芯序号下拉框
                    RefreshCellTemplates();

                    // 自动执行预览
                    ExecutePreview();
                }
                else
                {
                    Logger.Warn($"添加模组失败");
                    MessageBox.Show($"保存失败");
                }
            }
            catch (Exception ex)
            {
               // LogHelper.Errors("保存模组失败");
                MessageBox.Show($"保存失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 取消操作
        /// </summary>
        private void ExecuteCancel()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        /// <summary>
        /// 预览模组
        /// </summary>
        private void ExecutePreview()
        {
            try
            {
                if (_currentModuleId <= 0)
                {
                    MessageBox.Show("请先保存模组信息！");
                    return;
                }

                // 获取当前模组下的所有电芯
                var cells = DbContext.GetInstance().Queryable<Template_Cell>()
                               .Where(c => c.ModuleId == _currentModuleId)
                               .OrderBy(c => c.CellIndex)
                               .ToList();
                if (cells != null && cells.Count > 0)
                {
                    // 更新模组显示
                    ModuleDisplay.UpdateDisplay(cells);
                    Logger.Info("执行模组预览成功");
                }
            }
            catch (Exception ex)
            {
              //  LogHelper.Errors("预览失败");
                MessageBox.Show($"预览失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 保存电芯模板信息
        /// </summary>
        private void ExecuteSaveCell()
        {
            try
            {
                if (SelectedCellTemplate == null)
                {
                    MessageBox.Show("请选择要保存的电芯！");
                    return;
                }

                // 更新电芯模板属性
                SelectedCellTemplate.Polarity = IsPositive;

                // 根据电芯极性确定胶条位置
                SelectedCellTemplate.Side1Tape = (CellFaceTapeType)FrontTape;
                SelectedCellTemplate.Side2Tape = BackTape;



                // 保存电芯模板
                var result = DbContext.GetInstance().Updateable<Template_Cell>(SelectedCellTemplate).UpdateColumns(c => new { c.Side1Tape, c.Side2Tape, c.Polarity }).ExecuteCommand() > 0;


                if (result)
                {
                    Logger.Info($"成功保存电芯模板：序号{SelectedCellTemplate.CellIndex}, " +
                              $"极性:{(IsPositive ? "正极" : "负极")}, " +
                              $"Side1胶条:{SelectedCellTemplate.Side1Tape}, " +
                              $"Side2胶条:{SelectedCellTemplate.Side2Tape}");

                    // 刷新预览
                    ExecutePreview();
                }
                else
                {
                    Logger.Warn($"保存电芯模板失败");
                    MessageBox.Show("保存失败！");
                }
            }
            catch (Exception ex)
            {
              //  LogHelper.Errors("保存电芯模板失败");
                MessageBox.Show($"保存失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 刷新电芯模板集合
        /// </summary>
        private void RefreshCellTemplates()
        {
            try
            {
                CellTemplates.Clear();
                var cells = DbContext.GetInstance().Queryable<Template_Cell>()
                    .Where(c => c.ModuleId == _currentModuleId)
                    .OrderBy(c => c.CellIndex)
                    .ToList();
                if (cells != null)
                {
                    foreach (var cell in cells)
                    {
                        CellTemplates.Add(cell);
                    }
                }
                Logger.Info("刷新电芯模板集合成功");
            }
            catch (Exception ex)
            {
               // LogHelper.Errors("刷新电芯模板集合失败");
            }
        }
    }
}