using Ctp0600P.Client.ViewModels.StationTaskViewModels;

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

namespace Ctp0600P.Client.Views.StationTaskPages
{
    /// <summary>
    /// RecordTimeTaskPage.xaml 的交互逻辑
    /// </summary>
    public partial class RecordTimeTaskPage : StationTaskCommonPage
    {
        public RecordTimeViewModel _vm;
        public RecordTimeTaskPage(RecordTimeViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            this.DataContext = _vm;
        }
    }
}
