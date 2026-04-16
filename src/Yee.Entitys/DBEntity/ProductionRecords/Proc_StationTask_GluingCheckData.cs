using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.Common;

namespace Yee.Entitys.DBEntity
{
    [Table("Proc_StationTask_GluingCheckData")]
    public class Proc_StationTask_GluingCheckData : CommonData
    {
        public Proc_StationTask_Main? Proc_StationTask_Main { get; set; }

        public StationTaskStatusEnum? Status { get; set; }

        public string? UpMesCode { get; set; }

        [MaxLength(200)]
        public string PackPN { get; set; }

        public int? StationId { get; set; }
        public int? StepId { get; set; }

        public int Number { get; set; }
    }
}
