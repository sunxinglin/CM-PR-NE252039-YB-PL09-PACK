
using Catl.WebServices.MIFindCustomAndSfcData;

namespace Ctp0600P.Shared.CatlMes
{

    public class MIFindCustomAndSfcDataConfig
    {
        public CatlMesConnectionParams ConnectionParams { get; set; }
        public MIFindCustomAndSfcParams InterfaceParams { get; set; }
    }
    public class MIFindCustomAndSfcParams : CatlMesConfigurationBase
    {
        public bool findSfcByInventory { get; set; }
        public string sfcOrder { get; set; }
        public string targetOrder { get; set; }
        public modeProcessSFC modeProcessSfc { get; set; }
        public string sfc { get; set; }
        public string inventory { get; set; }
        public string Resource { get; set; }
        public string dcGroup { get; set; }
        public string dcGroupRevision { get; set; }

        public ObjectAliasEnum category { get; set; }

        public string dataField { get; set; }
        public string usage { get; set; }


        public ObjectAliasEnum masterDataArray { get; set; }

    }
}
