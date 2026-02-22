using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogerTech.Common.Models
{
    [SugarIndex("index_codetable1_name", nameof(LogModel.RecordTime), OrderByType.Desc)]
    public class LogModel
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity =true)]
        public int  ID { get; set; }
        public DateTime RecordTime { get; set; }

        public string LogType { get; set; }
        public string InterfaceName { get; set; }

        [SugarColumn(IsNullable = true)]
        public string ProductID { get; set; }
        [SugarColumn(Length = 500)]
        public string Message { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Result { get; set; }
    }
}
