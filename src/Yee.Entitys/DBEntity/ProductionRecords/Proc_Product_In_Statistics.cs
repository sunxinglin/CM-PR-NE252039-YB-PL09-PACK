using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Entitys.Common;
using Yee.Entitys.Production;

namespace Yee.Entitys.DBEntity.ProductionRecords
{
    /// <summary>
    /// 产品入库表
    /// </summary>
    [Table("proc_product_in_statistics")]
    public class Proc_Product_In_Statistics : CommonData
    {
        /// <summary>
        /// 产品条码
        /// </summary>
        public string ProductCode { get; set; }
        /// <summary>
        /// 入库时间
        /// </summary>
        public DateTime InstorageTime { get; set; }
        public int StationId { get; set; }
        public Base_Station? Station { get; set; }
        public Proc_ProductStates State { get; set; }
    }

}
