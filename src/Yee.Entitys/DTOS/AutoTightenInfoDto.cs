using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yee.Entitys.DTOS
{
    public class AutoTightenInfoDto
    {
        public int Limit { get; set; } = 20;
        public int Page { get; set; } = 1;
        public string? PackCode { get; set; }
        
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }

        //public string AutoBoltType { get; set; } = "Lid";
    }
}
