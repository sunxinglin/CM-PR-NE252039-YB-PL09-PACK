using Ctp0600P.Client.ViewModels.StationTaskViewModels;

namespace Ctp0600P.Client.Views.StationTaskPages
{
    /// <summary>
    /// ScanCode.xaml 的交互逻辑
    /// </summary>
    public partial class StewingTimeout : StationTaskCommonPage
    {
        public StewingTimeoutViewModel _VM;

        public StewingTimeout(StewingTimeoutViewModel vm) : base()
        {
            InitializeComponent();
            _VM = vm;
            this.DataContext = _VM;
        }
    }
}