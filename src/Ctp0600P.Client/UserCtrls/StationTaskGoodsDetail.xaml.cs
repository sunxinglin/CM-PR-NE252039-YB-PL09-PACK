using FutureTech.Mvvm;
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

namespace Ctp0600P.Client.UserCtrls
{
    /// <summary>
    /// StationTaskDetail.xaml 的交互逻辑
    /// </summary>
    public partial class StationTaskGoodsDetail : UserControl
    {
        public StationTaskGoodsDetailVm VM;
        public StationTaskGoodsDetail()
        {
            InitializeComponent();
        }
    }


    public class StationTaskGoodsDetailVm : ViewModelBase
    {
        public string GoodsName { get; set; }
        public string GoodsPN { get; set; }
        public string NeedNum { get; set; }
        public string Status { get; set; }
        public string GoodsBatch { get; set; }
    }
}
