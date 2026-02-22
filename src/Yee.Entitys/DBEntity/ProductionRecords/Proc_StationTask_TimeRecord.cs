using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yee.Entitys.Common;

namespace Yee.Entitys.DBEntity.ProductionRecords
{
    [Table("Proc_StationTask_TimeRecord")]
    public class Proc_StationTask_TimeRecord : CommonData
    {
        public string TimeValue { get; set; } = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        public string UploadMesCode { get; set; } = "";

        public string TimeFlag { get; set; } = "";

        public string SerialCode { get; set; } = "";

        public string StationCode { get; set; } = "";

        public Proc_StationTask_Record? Proc_StationTask_Record { get; set; }
        public int Proc_StationTask_RecordId { get; set; }
    }
}
