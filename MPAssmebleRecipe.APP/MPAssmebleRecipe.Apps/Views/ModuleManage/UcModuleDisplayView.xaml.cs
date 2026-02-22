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
using MPAssmebleRecipe.Apps.ViewModels.ModuleManage;

namespace MPAssmebleRecipe.Apps.Views.ModuleManage
{
    /// <summary>
    /// UcModuleDisplayView.xaml 的交互逻辑
    /// </summary>
    public partial class UcModuleDisplayView : UserControl
    {
        public UcModuleDisplayView()
        {
            InitializeComponent();
            
            this.Loaded += UcModuleDisplayView_Loaded;
        }

        private void UcModuleDisplayView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is UcModuleDisplayViewModel vm)
            {
                vm.Initialize(MainContainer);
            }
        }
    }
}
