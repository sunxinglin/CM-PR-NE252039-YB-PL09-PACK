
using Catl.WebServices.MachineIntegrationServices;

namespace Ctp0600P.Shared.CatlMes
{

    public class DataCollectForMoudleTestConfig
    {
        public CatlMesConnectionParams ConnectionParams { get; set; }
        public DataCollectForMoudleTestParams InterfaceParams { get; set; }
    }
    public class DataCollectForMoudleTestParams : CatlMesConfigurationBase
    {
        public ModeProcessSfc modeProcessSFC { get; set; }
        public string sfc { get; set; }
        public string Resource { get; set; }
        public string dcGroup { get; set; }
        public string dcGroupRevision { get; set; }
    }

}
