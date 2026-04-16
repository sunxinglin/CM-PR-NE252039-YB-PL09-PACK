using Ctp0600P.Client.ViewModels.StationTaskViewModels;

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
