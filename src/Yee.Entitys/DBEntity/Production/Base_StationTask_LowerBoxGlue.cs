using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Yee.Entitys.Common;
using Yee.Entitys.Production;

namespace Yee.Entitys.DBEntity.Production
{
    [Table("Base_StationTask_LowerBoxGlue")]
    public class Base_StationTask_LowerBoxGlue : CommonData
    {
        public Base_StationTask? StationTask { get; set; }
        public int? StationTaskId { get; set; }

        [MaxLength(200)]
        public string ParameterName { get; set; } = null!;

        public string UpMesCode { get; set; } = null!;

        [Column(TypeName = "decimal(18,6)")]
        public decimal? MinValue { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal? MaxValue { get; set; }

    }
}
