using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Yee.Entitys.DBEntity.Common;

namespace Yee.Entitys.Production
{
    [Table("Base_StationTask_AutoModuleInBox")]
    public class Base_StationTask_AutoModuleInBox : BaseDataModel
    {
        [MaxLength(200)]
        public string ParameterName { get; set; } = "";

        public Base_StationTask? StationTask { get; set; }
        public int? StationTaskId { get; set; }

        public ModuleInBoxDataTypeEnum ModuleInBoxDataType { get; set; }

        public int Location { get; set; } = 1;

        public string ModulePN { get; set; } = "";

        //public int ModuleType { get; set; }= 1;

        [Column(TypeName = "decimal(10,3)")]
        public decimal MinValue { get; set; }

        [Column(TypeName = "decimal(10,3)")]
        public decimal MaxValue { get; set; }
        public string UpMesCode { get; set; } = "";
    }


    public enum ModuleInBoxDataTypeEnum
    {
        模组码 = 1,
        模组长度 = 2,
        保压时间 = 3,
        下压距离 = 4,
        下压压力 = 5,
        左侧压力 = 6,
        右侧压力 = 7,
        入箱完成时间 = 8,
        模组入箱时长 = 9
    }
}
