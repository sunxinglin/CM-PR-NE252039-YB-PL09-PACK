using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctp0600P.Shared
{
    public class CatlMesOpt
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Ini配置文件路径
        /// </summary>
        public string IniFileDir { get; set; }

        /// <summary>
        /// Ini配置文件路径
        /// </summary>
        public string IniFileName { get; set; } = "MESCFG.ini";

        /// <summary>
        /// 进站
        /// </summary>
        public string MIFindCustomAndSfcDataInterfaceName { get; set; } = "miFindCustomAndSfcData";

        /// <summary>
        /// 首件
        /// </summary>
        public string DataCollectForResourceFAIInterfaceName { get; set; } = "dataCollectForResourceFAI";
        public string DataCollectForResourceInspectTaskName { get; set; } = "dataCollectForResourceInspectTask";

        /// <summary>
        /// 检验
        /// </summary>
        public string MICheckSFCStatusExInterfaceName { get; set; } = "miCheckSFCStatusEx";

        /// <summary>
        /// 检验贴纸及库存
        /// </summary>
        public string MICheckBOMInventoryInterfaceName { get; set; } = "miCheckBOMInventory";
        /// <summary>
        /// 组装+收数
        /// </summary>
        public string MiAssembleAndCollectDataForSfcInterfaceName { get; set; } = "miAssmebleAndCollectDataForSfc";

        /// <summary>
        /// 收数
        /// </summary>
        public string DataCollectForMoudleTestInterfaceName { get; set; } = "dataCollectForMoudleTest";

        /// <summary>
        /// 组装
        /// </summary>
        public string MiAssembleComponentToSfcName { get; set; } = "miAssembleComponentToSfc";


        public string DataCollectForResourceInspect { get; set; } = "dataCollectForResourceInspect";

        public string FindSfcByInventoryName { get; set; } = "findSfcByInventoryName";

        public string MiCheckInventoryAttributesName { get; set; } = "miCheckInventoryAttributes";

        public string GetParametricValueName { get; set; } = "getParametricValue";

    }
}
