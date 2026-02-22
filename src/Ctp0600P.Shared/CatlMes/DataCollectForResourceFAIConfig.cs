
using Catl.WebServices.MIFindCustomAndSfcData;

namespace Ctp0600P.Shared.CatlMes
{

    public class DataCollectForResourceFAIConfig
    {
        public CatlMesConnectionParams ConnectionParams { get; set; }
        public DataCollectForResourceFAIParams InterfaceParams { get; set; }
    }
    public class DataCollectForResourceFAIParams : CatlMesConfigurationBase
    {

        public dcModeEnum dcMode { get; set; }
        public string sfc { get; set; }
        public string Resource { get; set; }
        public string dcGroup { get; set; }
        public string dcGroupRevision { get; set; }
        public string dcGroupSequence { get; set; }
        public string material { get; set; }
        public string materialRevision { get; set; }
        
    }

    public enum dcModeEnum
    {
        /// <summary>
        /// 指定数据收集组
        /// </summary>
        GIVEN_DCG,
        /// <summary>
        ///    指定SFC
        /// </summary>
        SFC_DCG,
        /// <summary>
        /// 指定物料
        /// </summary>
        ITEM_DCGC,
        /// <summary>
        /// 自动获取
        /// </summary>
        Auto_DCG
    }
}
