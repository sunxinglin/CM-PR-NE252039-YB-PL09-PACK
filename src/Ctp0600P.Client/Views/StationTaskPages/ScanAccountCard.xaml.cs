
using Ctp0600P.Client.ViewModels.StationTaskViewModels;

namespace Ctp0600P.Client.Views.StationTaskPages
{
    /// <summary>
    /// ScanCode.xaml 的交互逻辑
    /// </summary>
    public partial class ScanAccountCard : StationTaskCommonPage
    {
        public ScanAccountCardViewModel _VM;

        public ScanAccountCard(ScanAccountCardViewModel vm) : base()
        {
            InitializeComponent();
            _VM = vm;
            this.DataContext = _VM;
        }
    }
}