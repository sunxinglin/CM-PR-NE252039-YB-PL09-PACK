
using Ctp0600P.Client.ViewModels.StationTaskViewModels;


namespace Ctp0600P.Client.Views.StationTaskPages
{
    /// <summary>
    /// LeakPage.xaml 的交互逻辑
    /// </summary>
    public partial class LeakPage : StationTaskCommonPage
    {
        public LeakViewModel _VM;

        public LeakPage(LeakViewModel vm) : base()
        {
            InitializeComponent();
            _VM = vm;
            this.DataContext = _VM;
        }

    }
}