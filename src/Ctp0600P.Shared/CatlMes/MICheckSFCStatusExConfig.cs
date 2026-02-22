
using Catl.WebServices.MICheckSFCStatusEx;

namespace Ctp0600P.Shared.CatlMes
{

    public class MICheckSFCStatusExConfig
    {
        public CatlMesConnectionParams ConnectionParams { get; set; }
        public MICheckSFCStatusExParams InterfaceParams { get; set; }
    }
    public class MICheckSFCStatusExParams : CatlMesConfigurationBase
    {
        public string sfc { get; set; }
        public string IsGetSFCFromCustomerBarcode { get; set; }
    }
}
