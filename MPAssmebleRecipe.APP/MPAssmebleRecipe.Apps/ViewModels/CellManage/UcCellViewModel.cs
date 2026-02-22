using RogerTech.Common.Models;
using Prism.Mvvm;
using System.Windows.Media;

namespace MPAssmebleRecipe.Apps.ViewModels.CellManage
{
    /// <summary>
    /// 电芯显示视图模型
    /// </summary>
    public class UcCellViewModel : BindableBase
    {
        /// <summary>
        /// 更新电芯显示
        /// </summary>
        /// <param name="cell">电芯模板</param>
        /// <remarks>
        /// 1. Polarity为True时，正极在右侧
        /// 2. Polarity为False时，负极在右侧
        /// 3. Side1Tape不为None时，显示上方胶条
        /// 4. Side2Tape不为None时，显示左侧胶条
        /// </remarks>
        public void UpdateFromCell(Template_Cell cell)
        {
            if (cell == null) return;

            CellIndex = cell.CellIndex;

            // 更新极性显示
            if (cell.Polarity)
            {
                // 正极在右
                LeftPoleBackground = Brushes.Gray;
                LeftPoleSymbol = "-";
                LeftPoleForeground = Brushes.White;

                RightPoleBackground = Brushes.Gray;
                RightPoleSymbol = "+";
                RightPoleForeground = Brushes.Black;
            }
            else
            {
                // 负极在右
                LeftPoleBackground = Brushes.Gray;
                LeftPoleSymbol = "+";
                LeftPoleForeground = Brushes.Black;

                RightPoleBackground = Brushes.Gray;
                RightPoleSymbol = "-";
                RightPoleForeground = Brushes.White;
            }
            // 更新胶条状态
            HasTopTape = cell.Side1Tape != CellFaceTapeType.不贴;
            HasLeftTape = cell.Side2Tape != CellSideTapeType.不贴;
        }

        #region Properties

        private string _leftPoleSymbol;
        /// <summary>
        /// 左侧极柱符号
        /// </summary>
        public string LeftPoleSymbol
        {
            get => _leftPoleSymbol;
            set => SetProperty(ref _leftPoleSymbol, value);
        }

        private string _rightPoleSymbol;
        /// <summary>
        /// 右侧极柱符号
        /// </summary>
        public string RightPoleSymbol
        {
            get => _rightPoleSymbol;
            set => SetProperty(ref _rightPoleSymbol, value);
        }

        private Brush _leftPoleBackground;
        /// <summary>
        /// 左侧极柱背景色
        /// </summary>
        public Brush LeftPoleBackground
        {
            get => _leftPoleBackground;
            set => SetProperty(ref _leftPoleBackground, value);
        }

        private Brush _rightPoleBackground;
        /// <summary>
        /// 右侧极柱背景色
        /// </summary>
        public Brush RightPoleBackground
        {
            get => _rightPoleBackground;
            set => SetProperty(ref _rightPoleBackground, value);
        }

        private Brush _leftPoleForeground;
        /// <summary>
        /// 左侧极柱前景色
        /// </summary>
        public Brush LeftPoleForeground
        {
            get => _leftPoleForeground;
            set => SetProperty(ref _leftPoleForeground, value);
        }

        private Brush _rightPoleForeground;
        /// <summary>
        /// 右侧极柱前景色
        /// </summary>
        public Brush RightPoleForeground
        {
            get => _rightPoleForeground;
            set => SetProperty(ref _rightPoleForeground, value);
        }

        private bool _hasTopTape;
        /// <summary>
        /// 是否显示上方胶条
        /// </summary>
        public bool HasTopTape
        {
            get => _hasTopTape;
            set => SetProperty(ref _hasTopTape, value);
        }

        private bool _hasLeftTape;
        /// <summary>
        /// 是否显示左侧胶条
        /// </summary>
        public bool HasLeftTape
        {
            get => _hasLeftTape;
            set => SetProperty(ref _hasLeftTape, value);
        }

        private int _cellIndex;
        /// <summary>
        /// 电芯序号
        /// </summary>
        public int CellIndex
        {
            get => _cellIndex;
            set => SetProperty(ref _cellIndex, value);
        }

        #endregion
    }
}