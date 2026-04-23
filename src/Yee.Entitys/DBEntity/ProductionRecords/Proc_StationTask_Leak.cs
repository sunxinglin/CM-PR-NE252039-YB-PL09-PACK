using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.Common;

namespace Yee.Entitys.DBEntity
{
    [Table("Proc_StationTask_Leak")]
    public class Proc_StationTask_Leak : CommonData
    {
        /// <summary>
        /// 关联工位恩物主表
        /// </summary>
        public Proc_StationTask_Record? StationTask_Record { get; set; }
        public int StationTask_RecordId { get; set; }

        public StationTaskStatusEnum Status { get; set; }

        [NotMapped]
        public List<Proc_StationTask_LeakDetail> Proc_StationTask_LeakDetails { get; set; }
    }
}