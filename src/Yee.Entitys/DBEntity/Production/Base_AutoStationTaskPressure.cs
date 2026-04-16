using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Yee.Entitys.Common;

namespace Yee.Entitys.Production
{
    [Table("Base_StationTaskPressure")]
    public class Base_AutoStationTaskPressure : CommonData
    {
        [MaxLength(100)]
        public string ParameterName { get; set; } = "";

        public Base_StationTask? StationTask { get; set; }
        public int? StationTaskId { get; set; }

        public PressurizeDataType PressurizeDataType { get; set; }

        public string UpMesCode { get; set; } = "";

        public int PressureLocate { get; set; } = 1;

        [Column(TypeName = "decimal(10,3)")]
        public decimal MinValue { get; set; } = 0;

        [Column(TypeName = "decimal(10,3)")]
        public decimal MaxValue { get; set; }
    }

    public enum PressurizeDataType
    {

        肩部高度 = 0,
        保压时长 = 1,
        平均压力 = 2,
        最大压力 = 3,
    }

}
