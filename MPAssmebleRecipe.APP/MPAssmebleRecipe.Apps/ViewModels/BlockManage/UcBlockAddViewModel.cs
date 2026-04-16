using DryIoc;
using MPAssmebleRecipe.Logger.Interfaces;
using OfficeOpenXml.VBA;

//using MPAssmebleRecipe.Service.IService;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RogerTech.Common;
using RogerTech.Common.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace MPAssmebleRecipe.Apps.ViewModels.BlockManage
{
    /// <summary>
    /// Block添加视图模型
    /// </summary>
    public class UcBlockAddViewModel : BindableBase, IDialogAware
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
        #region Properties
        public string Title => "添加Block";

        private string _packPn;
        public string PackPn
        {
            get => _packPn;
            set => SetProperty(ref _packPn, value);
        }
        private int _packId;
        public int PackId
        {
            get => _packId;
            set => SetProperty(ref _packId, value);
        }

        private string _blockname;
        public string BlockName
        {
            get => _blockname;
            set => SetProperty(ref _blockname, value);
        }
        private string _blockpn;
        public string BlockPn
        {
            get => _blockpn;
            set => SetProperty(ref _blockpn, value);
        }
        private string _attributionline = "ABCD";
        public string AttributionLine
        {
            get => _attributionline;
            set => SetProperty(ref _attributionline, value);
        }

        /// <summary>
        /// Pack中Block位置
        /// </summary>
        private int _blocksequence;
        public int BlockSequence
        {
            get => _blocksequence;
            set => SetProperty(ref _blocksequence, value);
        }
        /// <summary>
        /// 小车中Block序号
        /// </summary>
        private int _blockindex;
        public int BlockIndex
        {
            get => _blockindex;
            set => SetProperty(ref _blockindex, value);
        }
        /// <summary>
        /// 小车中Block数量
        /// </summary>
        private int _blockAmount;
        public int BlockAmount
        {
            get => _blockAmount;
            set => SetProperty(ref _blockAmount, value);
        }

        /// <summary>
        /// 模组类型
        /// </summary>
        private ModuleType _moduleType = ModuleType.Pillar;
        public ModuleType ModuleTypes
        {
            get => _moduleType;
            set => SetProperty(ref _moduleType, value);
        }
        /// <summary>
        /// 模组数量
        /// </summary>
        private int _moduleAmount;
        public int ModuleAmount
        {
            get => _moduleAmount;
            set => SetProperty(ref _moduleAmount, value);
        }
        /// <summary>
        /// 模组电芯数量
        /// </summary>
        private int cellAmount;
        public int CellAmount
        {
            get { return cellAmount; }
            set { cellAmount = value; }
        }

        /// <summary>
        /// Pack层号
        /// </summary>
        private int _layernum = 1;
        public int LayerNum
        {
            get => _layernum;
            set => SetProperty(ref _layernum, value);
        }

        /// <summary>
        /// 极性
        /// </summary>
        private bool polarity = true;
        public bool Polarity
        {
            get { return polarity; }
            set { polarity = value; }
        }
        /// <summary>
        /// 限位片
        /// </summary>
        private bool limitplate = false;
        public bool LimitPlate
        {
            get { return limitplate; }
            set { limitplate = value; }
        }

        #endregion

        #region Commands
        public DelegateCommand ConfirmCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public event Action<IDialogResult> RequestClose;
        #endregion

        public void OnDialogOpened(IDialogParameters parameters)
        {
            PackId = parameters.GetValue<int>("PackId");
            PackPn = parameters.GetValue<string>("PackPn");
            if (PackId < 1 || string.IsNullOrEmpty(PackPn))
                return;
        }
        public bool CanCloseDialog()
        {
            return true;
        }
        public void OnDialogClosed()
        {
            Dispose();
        }

        public UcBlockAddViewModel(ILoggerHelper logger, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _closeToken = _eventAggregator.GetEvent<CloseAllDialogsEvent>().Subscribe(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel)));

            //  _blockService = blockService;
            _logger = logger;
            ConfirmCommand = new DelegateCommand(ExecuteConfirm, CanExecuteConfirm)
                .ObservesProperty(() => BlockName)
                .ObservesProperty(() => BlockPn)
                .ObservesProperty(() => AttributionLine)
                .ObservesProperty(() => BlockSequence)
                .ObservesProperty(() => BlockIndex)
                .ObservesProperty(() => BlockAmount)
                .ObservesProperty(() => ModuleAmount)
                .ObservesProperty(() => CellAmount)
                .ObservesProperty(() => LayerNum);
            CancelCommand = new DelegateCommand(ExecuteCancel);
        }

        private bool CanExecuteConfirm()
        {
            return !string.IsNullOrWhiteSpace(BlockName) &&
                   !string.IsNullOrWhiteSpace(BlockPn) &&
                   !string.IsNullOrWhiteSpace(AttributionLine) &&
                   !string.IsNullOrWhiteSpace(PackPn) &&
                   PackId > 0 &&
                   BlockSequence > 0 &&
                   BlockIndex > 0 &&
                   BlockAmount > 0 &&
                   ModuleAmount > 0 &&
                   CellAmount > 0 &&
                   LayerNum > 0 ;
        }
        int moduleIndex = 0, cellIndex = 0;
        private void ExecuteConfirm()
        {
            try
            {
                //判断AGV+小车中Block序号是否已经存在
                bool isany = DbContext.GetInstance().Queryable<Template_Block>().Any(r => r.BlockSequence == BlockSequence && r.BlockIndex == BlockIndex && r.PackId == PackId);
                if (isany)
                {
                    MessageBox.Show($"AGV托盘号[{BlockSequence}]，小车中Block序号[{BlockIndex}]，已存在", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                //判断相同位置下Block数量、Unit数量、电芯数量是否和之前的相同
                Template_Block template_Block = DbContext.GetInstance().Queryable<Template_Block>().First(r => r.BlockSequence == BlockSequence && r.PackId == PackId);
                if (template_Block != null) 
                {
                    if (template_Block.BlockAmount != BlockAmount)
                    {
                        MessageBox.Show($"Pack中Block位置[{BlockSequence}]下，小车中Block数量[{template_Block.BlockAmount}]与输入[{BlockAmount}]不匹配", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    if (template_Block.ModuleAmount != ModuleAmount)
                    {
                        MessageBox.Show($"Pack中Block位置[{BlockSequence}]下，Unit数量[{template_Block.ModuleAmount}]与输入[{ModuleAmount}]不匹配", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    moduleIndex = DbContext.GetInstance().Queryable<Template_Module>().Where(r => r.ModuleUid == template_Block.BlockUid).Max(r => r.ModuleIndex);
                    cellIndex = DbContext.GetInstance().Queryable<Template_Cell>().Where(r => r.CellUid == template_Block.BlockUid).Max(r => r.CellIndex);
                    Template_Module template_Module = DbContext.GetInstance().Queryable<Template_Module>().First(r => r.BlockId == template_Block.Id);
                    if (template_Module != null)
                    {
                        if (template_Module.CellAmount != CellAmount)
                        {
                            MessageBox.Show($"Pack中Block位置[{BlockSequence}]下，电芯数量[{template_Module.CellAmount}]与输入[{CellAmount}]不匹配", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }

                }

                Template_Block blockTemplate = new Template_Block();
                blockTemplate.BlockUid = template_Block == null ? Guid.NewGuid().ToString() : template_Block.BlockUid;
                blockTemplate.BlockName = BlockName;
                blockTemplate.BlockPn = BlockPn;
                blockTemplate.PackId = PackId;
                blockTemplate.ParentPn = PackPn;
                blockTemplate.AttributionLine = AttributionLine;
                blockTemplate.BlockSequence = BlockSequence;
                blockTemplate.BlockIndex = BlockIndex;
                blockTemplate.BlockAmount = BlockAmount;
                blockTemplate.ModuleAmount = ModuleAmount;
                blockTemplate.LayerNum = LayerNum;
                blockTemplate.LineNum = 1;
                blockTemplate.LimitPlate = LimitPlate;
                blockTemplate.SplitType = false;
                blockTemplate.ModuleItems = new List<Template_Module>();

                //创建模组和电芯信息
                if (ModuleTypes == ModuleType.Pillar || ModuleTypes == ModuleType.Beam)
                {
                    for (int i = 0; i < ModuleAmount; i++)
                    {
                        moduleIndex++;
                        Template_Module moduleTemplate = new Template_Module
                        {
                            ModuleUid = blockTemplate.BlockUid,
                            BlockPn = BlockPn,
                            BlockSequence = BlockSequence,
                            CellAmount = CellAmount,
                            ModuleIndex = moduleIndex,
                            ModuleType = ModuleTypes,
                            ModuleRotate = false,
                            Side1Tape = ModuleFaceTapeType.不贴,
                            Side2Tape = ModuleSideTapeType.不贴,
                            WaterCoolType = WaterCoolType.无,
                            WaterTubeType = WaterTubeType.无,
                            SulationCover = SulationCoverType.不贴,
                        };
                        if (ModuleTypes == ModuleType.Beam)
                            moduleTemplate.ModulePolarity = i % 2 == 0 ? Polarity : !Polarity;
                        else
                            moduleTemplate.ModulePolarity = Polarity;
                        //创建电芯
                        moduleTemplate.CellItems = CreateCellList(CellAmount, i, moduleTemplate.Id, blockTemplate.BlockUid);
                        blockTemplate.ModuleItems.Add(moduleTemplate);
                    }
                }

                IDialogParameters parameters = new DialogParameters()
                {
                    {"Blocks", blockTemplate},
                    {"ModuleTypes", ModuleTypes}
                };
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, parameters));
            }
            catch (Exception ex)
            {
               // LogHelper.Errors("添加Block失败");
                MessageBox.Show($"添加失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ExecuteCancel()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        private List<Template_Cell> CreateCellList(int cellCount, int moduleindex, int moduleid, string uid)
        {
            List<Template_Cell> cells = new List<Template_Cell>();
            for (int j = 0; j < cellCount; j++)
            {
                cellIndex++;
                Template_Cell cell = new Template_Cell
                {
                    CellUid = uid,
                    BlockPn = BlockPn,
                    ModuleId = moduleid,
                    CellIndex = cellIndex,
                };
                if (ModuleTypes == ModuleType.Pillar)
                {
                    //第一个电芯不贴，后面全部贴大面黑胶
                    cell.Side1Tape = j == 0 ? CellFaceTapeType.不贴 : CellFaceTapeType.黑色胶;
                    //按照电芯顺序交替正负极，Beam极性相同
                    cell.Polarity = j % 2 == 0 ? Polarity : !Polarity;
                    //第一个Beam不贴，后面全部贴小面胶
                    cell.Side2Tape = moduleindex == 0 ? CellSideTapeType.不贴 : CellSideTapeType.小面胶;
                }
                else if (ModuleTypes == ModuleType.Beam)
                {
                    //第一个Beam不贴，后面全部贴大面黑胶
                    cell.Side1Tape = moduleindex == 0 ? CellFaceTapeType.不贴 : CellFaceTapeType.黑色胶;
                    //按照Beam顺序交替正负极，Beam内电芯同一个极性
                    cell.Polarity = moduleindex % 2 == 0 ? Polarity : !Polarity;
                    //第一个电芯不贴，后面全部贴小面胶
                    cell.Side2Tape = j == 0 ? CellSideTapeType.不贴 : CellSideTapeType.小面胶;
                }
                cells.Add(cell);
            }
            return cells;
        }
    }
}