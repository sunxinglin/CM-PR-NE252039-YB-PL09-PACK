using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Common.Library;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.Common;
using Yee.Entitys.Production;

namespace Yee.Entitys.DBEntity
{
    [Table("Proc_StationTask_ScanAccountCard")]
    public class Proc_StationTask_ScanAccountCard : CommonData
    {
        /// <summary>
        /// 关联工位任务主表
        /// </summary>
        public Proc_StationTask_Record? StationTask_Record { get; set; }
        public int StationTask_RecordId { get; set; }
        public int? StationId { get; set; }
        public int? StepId { get; set; }

        /// <summary>
        /// Pack条码号
        /// </summary>
        [MaxLength(100)]
        public string? PackPN { get; set; }
        public StationTaskStatusEnum Status { get; set; }

        [MaxLength(200)]
        public string? ScanAccountCardName { get; set; }

        /// <summary>
        /// AccountValue
        /// </summary>
        [MaxLength(100)]
        public string? AccountValue { get; set; }

        [MaxLength(200)]
        public string? UpMesCode { get; set; }
    }
}
