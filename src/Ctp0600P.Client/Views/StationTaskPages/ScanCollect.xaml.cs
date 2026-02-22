
using Ctp0600P.Client.ViewModels.StationTaskViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;

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
