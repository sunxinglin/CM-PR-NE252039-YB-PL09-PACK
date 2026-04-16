using Catl.WebServices.AssembleAndCollectDataForSfc;

namespace Ctp0600P.Shared.CatlMes
{

    public class MIAssembleAndCollectDataForSfcConfig
    {
        public CatlMesConnectionParams ConnectionParams { get; set; }
        public MIAssembleAndCollectDataForSfcParams InterfaceParams { get; set; }
    }
    public class MIAssembleAndCollectDataForSfcParams : CatlMesConfigurationBase
    {
        public string sfc { get; set; }
        public string resource { get; set; }
        public dataCollectForSfcModeProcessSfc modeProcessSFC { get; set; }
        public bool partialAssembly { get; set; }
        public string dcGroup { get; set; }
        public string dcGroupRevision { get; set; }
    }
}
