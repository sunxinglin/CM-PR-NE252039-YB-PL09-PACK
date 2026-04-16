using System.Windows.Controls;

using FutureTech.Mvvm;

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
