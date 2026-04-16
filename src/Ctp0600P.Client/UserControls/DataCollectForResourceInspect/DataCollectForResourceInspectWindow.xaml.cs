using System.Windows;

using Ctp0600P.Client.CatlMes;

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
