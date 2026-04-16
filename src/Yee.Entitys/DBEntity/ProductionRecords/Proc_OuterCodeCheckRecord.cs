using System.ComponentModel.DataAnnotations.Schema;

using Yee.Entitys.Common;

namespace Yee.Entitys.DBEntity.ProductionRecords
{
    [Table("Proc_OuterCodeCheckRecord")]
    public class Proc_OuterCodeCheckRecord : CommonData
    {
        public string PackCode { get; set; } = null!;//Pack码
        public string ScanCode { get; set; } = null!;//扫码结果
        public string ComponentPN { get; set; } = null!;//物料PN
        public string CatlResponseCode { get; set; } = null!;//应装配的条码

        public string ElectricityBoxPN { get; set; } = null!;//高压盒PN

        public bool IsPass { get; set; } = false;//校验是否通过

        public PassType PassType { get; set; } = PassType.未通过;

       
    }

    public enum PassType
    {
        正常通过 = 0,
        强制通过 = 1,
        未通过 = 2
    }
}
