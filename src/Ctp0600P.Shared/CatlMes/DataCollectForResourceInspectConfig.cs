using Catl.WebServices.DataCollectForResourceInspect;

namespace Ctp0600P.Shared.CatlMes
{
    /// <summary>
    /// 首件接口参数
    /// </summary>
    public class DataCollectForResourceInspectConfig
    {
        public CatlMesConnectionParams ConnectionParams { get; set; }
        public DataCollectForResourceInspectParams InterfaceParams { get; set; }

    }
    public class DataCollectForResourceInspectParams : CatlMesConfigurationBase
    {
        public string Resource { get; set; }

        public string DcGroup { get; set; }
        public string DcGroupRevision { get; set; }
        public string DcMode { get; set; }
        public string DcSequence { get; set; }
        public string ExecuteMode { get; set; }
        public machineIntegrationParametricData parametricDatas { get; set; }
      
    }
}
