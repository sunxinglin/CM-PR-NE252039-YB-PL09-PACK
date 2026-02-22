using Ctp0600P.Client.CatlMes;
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
using System.Windows.Shapes;

namespace Ctp0600P.Client.UserControls.DataCollectForResourceInspect
{
    /// <summary>
    /// DataCollectForResourceInspectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DataCollectForResourceInspectWindow : Window
    {
        public DataCollectForResourceInspectWindow()
        {
            InitializeComponent();
        }
    }
    public class DataCollectForResourceInspectWindowViewModle
    {
        public DataCollectForResourceInspectWindowViewModle(DataCollectForResourceInspectVM dataCollectForResourceInspectVM)
        {
            DataCollectForResourceInspectVM = dataCollectForResourceInspectVM;
        }

        public DataCollectForResourceInspectVM DataCollectForResourceInspectVM { get; }
    }
}
