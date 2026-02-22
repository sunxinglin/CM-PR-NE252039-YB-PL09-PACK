using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yee.Entitys.Common;
using Yee.Entitys.Production;

namespace Yee.Entitys.DBEntity.Production
{
    [Table("Base_StationTask_TightenRework")]
    public class Base_StationTask_TightenRework : CommonData
    {
        public Base_StationTask? StationTask { get; set; }
        public int? StationTaskId { get; set; }

        public string TaskName { get; set; } = "";

        public TightenReworkType ReworkType { get; set; }
        public int ScrewNum { get; set; }//螺栓数量

        public int ProgramNo { get; set; } = 1;
        public string UpMesCode { get; set; } = "";

        public string DevicesNos { get; set; } = "1";

        [Column(TypeName ="decimal(10,3)")]
        public decimal MinTorque { get; set; } = 0;

        [Column(TypeName ="decimal(10,3)")]
        public decimal MaxTorque { get; set; } = 99;
        [Column(TypeName = "decimal(10,3)")]
        public decimal MinAngle { get; set; } = 0;
        [Column(TypeName = "decimal(10,3)")]
        public decimal MaxAngle { get; set; } = 999;
        
    }

    public enum TightenReworkType
    { 
        Module = 0,//模组
        Lid = 1,//上盖
    }

}
