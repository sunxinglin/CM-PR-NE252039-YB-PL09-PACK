using Ctp0600P.Client.ViewModels.StationTaskViewModels;

namespace Ctp0600P.Client.Views.StationTaskPages
{
    /// <summary>
    /// AnyLoad.xaml 的交互逻辑
    /// </summary>
    public partial class ScanCollect : StationTaskCommonPage
    {
        public ScanCollectViewModel _VM;
        public ScanCollect(ScanCollectViewModel vm)
        {
            InitializeComponent();
            _VM = vm;
            this.DataContext = _VM;
        }


        //public override void StationTaskCommonPage_Loaded(object sender, RoutedEventArgs e)
        //{
        //    App._ActivityAnyLoadPage = null;
        //    App._ActivityScanCodePage = null;
        //    App._ActivityBoltGunPage = null;
        //    App._ActivityRunAGVPage = null;
        //    App._ActivityRepairBoltGunCommon = null;
        //    App._ActivityScanAccountCard = null;
        //    App._ActivityUserScanPage = null;
        //    App._ActivityUserInputPage = null;

        //    App._ActivityUserScanPage = (ScanCollect)sender;
        //}

    }

   
}
