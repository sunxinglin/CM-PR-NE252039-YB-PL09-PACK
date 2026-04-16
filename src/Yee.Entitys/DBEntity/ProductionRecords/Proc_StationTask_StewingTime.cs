using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.Common;
using Yee.Entitys.Production;

namespace Yee.Entitys.DBEntity
{
    [Table("Proc_StationTask_StewingTime")]
    public class Proc_StationTask_StewingTime : CommonData
    {
        /// <summary>
        /// 关联工位任务主表
        /// </summary>
        public Proc_StationTask_Record? StationTask_Record { get; set; }
        public int StationTask_RecordId { get; set; }
        public Base_StationTask? StationTask { get; set; }
        public int? StationTaskId { get; set; }
        public StationTaskStatusEnum? Status { get; set; }

        [MaxLength(200)]
        public string? StewingTimeName { get; set; }

        /// <summary>
        /// 超时
        /// </summary>
        public bool Pass { get; set; }
        public string? UpMesCode { get; set; }
        public Decimal StewingTime { get; set; }

        public DateTime? StewingStartTime { get; set; }
        public DateTime? StewingCollectTime { get; set; }

        [MaxLength(200)]
        public string PackPN { get; set; }

        public int? StationId { get; set; }
        public int? StepId { get; set; }
        
    }
}
