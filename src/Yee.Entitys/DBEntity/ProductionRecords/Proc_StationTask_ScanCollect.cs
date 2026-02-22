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
    [Table("Proc_StationTask_ScanCollect")]
    public class Proc_StationTask_ScanCollect : CommonData
    {
        /// <summary>
        /// 关联工位恩物主表
        /// </summary>
        public Proc_StationTask_Record? StationTask_Record { get; set; }
        public int StationTask_RecordId { get; set; }

        public StationTaskStatusEnum Status { get; set; }

        [MaxLength(200)]
        public string? ScanCollectName { get; set; }

        /// <summary>
        /// 输入数据
        /// </summary>
        public string ScanCollectData { get; set; }
     
        [MaxLength(200)]
        public string? UpMesCode { get; set; }
        public Base_ProResource? Base_ProResource { get; set; }
        public int? Base_ProResourceId { get; set; }
        public int? StationId { get; set; }
        public int? StepId { get; set; }
        public string? PackPN { get; set; }
    }
}
