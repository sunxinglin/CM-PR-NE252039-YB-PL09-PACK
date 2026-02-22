using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yee.Entitys.Common;
using Yee.Entitys.Production;

namespace Yee.Entitys.DBEntity.Production
{
    [Table("Base_StationTask_RecordTime")]
    public class Base_StationTask_RecordTime : CommonData
    {
        [MaxLength(100)]
        public string RecordTimeTaskName { get; set; } = "";

        [MaxLength(30)]
        public string TimeOutFlag { get; set; } = "";

        [MaxLength(30)]
        public string UpMesCode { get; set; } = "";

        public Base_StationTask? Base_StationTask {  get; set; }
        
        public int? StationTaskId { get; set; }

    }
}
