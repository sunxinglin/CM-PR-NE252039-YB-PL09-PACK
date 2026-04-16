using System.ComponentModel.DataAnnotations.Schema;

using Yee.Entitys.Common;
using Yee.Entitys.Production;

namespace Yee.Entitys.DBEntity.ProductionRecords
{
    /// <summary>
    /// 产品入库表
    /// </summary>
    [Table("Proc_Product_Offline")]
    public class Proc_Product_Offline : CommonData
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
        public int? Productid { get; set; }
        public Base_Product? Product { get; set; }
        public int StepId { get; set; }
    }
    public enum Proc_ProductStates
    {
        正常下线=1,
        NG下线=2,
    }
}
