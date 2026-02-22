using MPAssmebleRecipe.Apps.ViewModels.CellManage;
//using MPAssmebleRecipe.Apps.Views.CellManage;
using RogerTech.Common.Models;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MPAssmebleRecipe.Apps.Views.CellManage;

namespace MPAssmebleRecipe.Apps.ViewModels.ModuleManage
{
    /// <summary>
    /// 模组显示视图模型
    /// </summary>
    public class UcModuleDisplayViewModel : BindableBase
    {
        private StackPanel _mainContainer;
        private List<Template_Cell> _currentModule;

        public void Initialize(StackPanel container)
        {
            _mainContainer = container;

            var parentGroupBox = container.Parent as GroupBox;
            if (parentGroupBox != null)
            {
                _mainContainer.Width = parentGroupBox.ActualWidth;
                _mainContainer.Height = parentGroupBox.ActualHeight;
            }
        }

        public void UpdateDisplay(List<Template_Cell> cellItems)
        {
            if (_mainContainer == null || cellItems == null || cellItems.Count == 0) return;

            _currentModule = cellItems;
            _mainContainer.Children.Clear();

            // 获取可用的显示区域
            double availableWidth = _mainContainer.ActualWidth;
            double availableHeight = _mainContainer.ActualHeight;

            // 计算每个电芯的尺寸
            int cellCount = cellItems.Count;
            double totalMargin = (cellCount + 1) * 4; // 上下间距共4像素

            // 计算每个电芯的实际高度（减去所有间距后平均分配）
            double cellHeight = (availableHeight - totalMargin) / cellCount;

            // 电芯宽度（留出左右边距）
            double cellWidth = availableWidth - 10;  // 左右各留5像素边距

            // 遍历模组中的所有电芯数据
            foreach (var cell in cellItems)
            {
                var cellView = new UcCellView
                {
                    Width = cellWidth,
                    Height = cellHeight,
                    Margin = new Thickness(5, 2, 5, 2)  // 上下左右留出间距
                };

                var cellVM = new UcCellViewModel();
                cellVM.UpdateFromCell(cell);
                cellView.DataContext = cellVM;

                _mainContainer.Children.Add(cellView);
            }
        }
    }
}