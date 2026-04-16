using Ctp0600P.Client.ViewModels.StationTaskViewModels;

namespace Ctp0600P.Client.Views.StationTaskPages
{
    /// <summary>
    /// TightenByImage.xaml 的交互逻辑
    /// </summary>
    public partial class TightenByImage : StationTaskCommonPage
    {
        public readonly TightenByImageViewModel _vm;
        public TightenByImage(TightenByImageViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            this.DataContext = _vm;
        }
    }
}