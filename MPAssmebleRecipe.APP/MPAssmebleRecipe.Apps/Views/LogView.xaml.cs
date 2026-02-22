using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MPAssmebleRecipe.Apps.ViewModels;
using RogerTech.Common.Models;
namespace MPAssmebleRecipe.Apps.Views
{
    /// <summary>
    /// LogView.xaml 的交互逻辑
    /// </summary>
    public partial class LogView : UserControl
    {
        public LogView()
        {
            InitializeComponent();
        }
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid == null) return;

            // 确保点击的对象是 DataGridCell
            var source = e.OriginalSource as DependencyObject;
            while (source != null && !(source is DataGridCell))
            {
                source = VisualTreeHelper.GetParent(source);
            }

            // 如果点击的不是 DataGridCell，则直接返回
            if (source == null) return;

            var selectedLogEntry = dataGrid.SelectedItem as LogModel;
            if (selectedLogEntry == null) return;

            var viewModel = DataContext as LogViewModel;
            if (viewModel == null) return;
            if (selectedLogEntry.Result == null) return;
            int code = -1;
            if (!int.TryParse(selectedLogEntry.Result, out code)) return;

            if (code < 100) return;
            viewModel.ShowCodeDetail(code);
        }
    }
}
