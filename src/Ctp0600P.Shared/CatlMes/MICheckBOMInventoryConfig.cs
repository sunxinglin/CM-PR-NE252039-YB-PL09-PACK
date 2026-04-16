using Catl.WebServices.MICheckBOMInventory;

namespace Ctp0600P.Shared.CatlMes
{

    public class MICheckBOMInventoryConfig
    {
        public CatlMesConnectionParams ConnectionParams { get; set; }
        public MICheckBOMInventoryParams InterfaceParams { get; set; }
    }
    public class MICheckBOMInventoryParams : CatlMesConfigurationBase
    {
        public string sfc { get; set; }
        public string M1_PN { get; set; }
        public string M2_PN { get; set; }
        public string M3_PN { get; set; }
        public string M4_PN { get; set; }
        public string resource { get; set; }

        public modeProcessSFC modeProcessSFC { get; set; }
        public bool modeCheckOperation { get; set; }
        public ObjectAliasEnum category { get; set; }

        public string dataField { get; set; }
        public string usage { get; set; }
        
    }
}
