using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Yee.Entitys.Common;

namespace Yee.Entitys.Production
{
    [Table("Base_StationTaskLowerBoxGlue")]
    public class Base_AutoStationTaskGlue : CommonData
    {
        [MaxLength(200)]
        public string ParameterName { get; set; } = "";

        public Base_StationTask? StationTask { get; set; }
        public int? StationTaskId { get; set; }

        public GlueType GlueType { get; set; }

        public string UpMesCode { get; set; } = "";

        public int GlueLocate { get; set; } = 1; //涂胶位置
        //public int ProgramNo { get; set; }

        [Column(TypeName = "decimal(10,3)")]
        public decimal MinValue { get; set; }

        [Column(TypeName = "decimal(10,3)")]
        public decimal MaxValue { get; set; }
    }

    public enum GlueType
    {
        A胶 = 1,
        B胶,
        胶比例,
        总胶重
    }
}
