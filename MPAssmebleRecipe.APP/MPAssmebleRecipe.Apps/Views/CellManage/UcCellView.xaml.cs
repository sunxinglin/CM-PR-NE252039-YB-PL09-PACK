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

namespace MPAssmebleRecipe.Apps.Views.CellManage
{
    /// <summary>
    /// UcCellView.xaml 的交互逻辑
    /// </summary>
    public partial class UcCellView : UserControl
    {
        public UcCellView()
        {
            InitializeComponent();
        }
        public UcCellView(int height, int width)
        {
            InitializeComponent();
            this.Height = height;
            this.Width = width;
        }
    }
}
