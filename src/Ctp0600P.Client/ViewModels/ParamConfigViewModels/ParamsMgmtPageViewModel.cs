using AsZero.DbContexts;
using Catl.HostComputer.Commons.Wpf;
using Ctp0600P.Client.CatlMes;
using Ctp0600P.Client.UserControls.DataCollectForResourceInspect;
using Ctp0600P.Client.ViewModels.StationTaskViewModels;
using FutureTech.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ctp0600P.Client.ViewModels
{
    public class ParamsMgmtPageViewModel : ViewModelBase
    {

        public ParamsMgmtPageViewModel(ScanCodeGunConfigPageVM scanCodeGunConfigPageVM,
            BoltGunConfigPageVM boltGunConfigPageVM,
            MIFindCustomAndSfcDataCtrlVM miFindCustomAndSfcDataCtrlVM,
            MICheckBOMInventoryCtrlVM mICheckBOMInventoryCtrlVM,
            MiAssembleAndCollectDataForSfcCtrlVM miAssembleAndCollectDataForSfcCtrlVM,
            DataCollectForMoudleTestCtrlVM dataCollectForMoudleTestCtrlVM,
            MiCheckSFCStatusExVM miCheckSFCStatusExVM,
            DataCollectForResourceInspectVM DataCollectForResourceInspectVM,
            DataCollectForResourceInspectListViewModle dataCollectForResourceInspectListViewModle,
            AnyLoadConfigPageVM anyLoadConfigPageVM)
        {
            ScanCodeGunConfigPageVM = scanCodeGunConfigPageVM;
            BoltGunConfigPageVM = boltGunConfigPageVM;
            AnyLoadConfigPageVM = anyLoadConfigPageVM;
            MIFindCustomAndSfcDataCtrlVM = miFindCustomAndSfcDataCtrlVM;
            MICheckBOMInventoryCtrlVM = mICheckBOMInventoryCtrlVM;
            MiAssembleAndCollectDataForSfcCtrlVM = miAssembleAndCollectDataForSfcCtrlVM;
            DataCollectForMoudleTestCtrlVM = dataCollectForMoudleTestCtrlVM;
            MiCheckSFCStatusExVM = miCheckSFCStatusExVM;
             DataCollectForResourceInspectListViewModle = dataCollectForResourceInspectListViewModle;
            this.DataCollectForResourceInspectVM = DataCollectForResourceInspectVM;
        }

        public ScanCodeGunConfigPageVM ScanCodeGunConfigPageVM { get; }
        public BoltGunConfigPageVM BoltGunConfigPageVM { get; }
        public AnyLoadConfigPageVM AnyLoadConfigPageVM { get; }
        
        public MIFindCustomAndSfcDataCtrlVM MIFindCustomAndSfcDataCtrlVM { get; }

        public DataCollectForResourceInspectVM DataCollectForResourceInspectVM { get; }
        public MICheckBOMInventoryCtrlVM MICheckBOMInventoryCtrlVM { get; }
        public MiAssembleAndCollectDataForSfcCtrlVM MiAssembleAndCollectDataForSfcCtrlVM { get; }
        public DataCollectForMoudleTestCtrlVM DataCollectForMoudleTestCtrlVM { get; }
        public DataCollectForResourceInspectListViewModle DataCollectForResourceInspectListViewModle { get; }

        public MiCheckSFCStatusExVM MiCheckSFCStatusExVM { get; }
    }
}
