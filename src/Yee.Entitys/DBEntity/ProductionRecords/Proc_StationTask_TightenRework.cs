using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yee.Entitys.Common;
using Yee.Entitys.DBEntity.Production;

namespace Yee.Entitys.DBEntity.ProductionRecords
{
    [Table("Proc_StationTask_TightenRework")]
    public class Proc_StationTask_TightenRework : CommonData
    {
        public Proc_StationTask_Record? StationTaskRecord{get; set;}
        public int StationTaskRecordId { get; set;}

        public TightenReworkType TightenReworkType { get; set;}

        public int OrderNo { get; set;}

        public bool ResultOk { get; set;}

        public int ProgramNo { get; set;}

        [Column(TypeName ="decimal(10,3)")]
        public decimal TorqueValue { get; set;}
        [Column(TypeName = "decimal(10,3)")]
        public decimal AngleValue { get; set;}

        [Column(TypeName = "decimal(10,3)")]
        public decimal TorqueMin { get; set;}

        [Column(TypeName = "decimal(10,3)")]
        public decimal TorqueMax { get; set;}
        [Column(TypeName = "decimal(10,3)")]
        public decimal AngleMin { get; set;}
        [Column(TypeName = "decimal(10,3)")]
        public decimal AngleMax { get; set;}


        public string UpMesCode { get; set; } = "";
        public string UpMesCodeJD { get; set; } = "";
        public string PackPn { get; set; } = "";
    }
}
