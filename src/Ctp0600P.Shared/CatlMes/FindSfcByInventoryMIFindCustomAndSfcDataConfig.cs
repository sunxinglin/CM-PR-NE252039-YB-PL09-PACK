using Catl.WebServices.MiFindCustomAndSfcDataServiceService;

namespace Ctp0600P.Shared.CatlMes
{
   
        /// <summary>
        /// 进站接口配置参数
        /// </summary>
        public class FindSfcByInventoryMIFindCustomAndSfcDataConfig
        {
            public CatlMesConnectionParams ConnectionParams { get; set; }
            public MiFindCustomAndSfcDataParamers InterfaceParams { get; set; }
        }
        public class MiFindCustomAndSfcDataParamers : CatlMesConfigurationBase
        {
            public string Sfc { get; set; }
            public string Resource { get; set; }
            public string Activity { get; set; }
            public string Inventory { get; set; }
            public ObjectAliasEnum MasterData { get; set; }
            public ObjectAliasEnum[] MasterDataArray { get; set; }
            public ObjectAliasEnum CategoryData { get; set; }
            public string DataField { get; set; }
            public customDataInParametricData[] CustomDataArray { get; set; }
            public bool FindSfcByInventory { get; set; }
            public bool IsGetXY { get; set; }
            public bool IsGetCSC { get; set; }
            public FmodeProcessSFC Mode { get; set; }
            public string SfcOrder { get; set; }
            public string TargetOrder { get; set; }
            public string CheckInventoryAB { get; set; }
            public bool ShowMarking { get; set; }
            public bool ShowMarkingSpecified { get; set; }
            public string FindInventoryByGbt { get; set; }
        }

    
}
