using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.Common;
using Yee.Entitys.Production;

namespace Yee.Entitys.DBEntity
{
    [Table("Proc_StationTask_CheckTimeout")]
    public class Proc_StationTask_CheckTimeout : CommonData
    {
        /// <summary>
        /// 关联工位恩物主表
        /// </summary>
        public Proc_StationTask_Record? StationTask_Record { get; set; }
        public int StationTask_RecordId { get; set; }
        public Base_StationTask? StationTask { get; set; }
        public int? StationTaskId { get; set; }
        public StationTaskStatusEnum? Status { get; set; }

        [MaxLength(200)]
        public string? TimeName { get; set; }

        /// <summary>
        /// 超时
        /// </summary>
        public bool Pass { get; set; }
        public string? UpMesCode { get; set; }
        public Decimal Time { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? CollectTime { get; set; }

        [MaxLength(200)]
        public string? PackPN { get; set; }

        public int? StationId { get; set; }
        public int? StepId { get; set; }
    }

  
}
