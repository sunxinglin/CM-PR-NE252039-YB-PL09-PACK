using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yee.Entitys.DTOS
{
    public class ModuleInBoxInfosDto
    {

        public int Limit { get; set; }
        public int Page { get; set; }
        public string? PackPN { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }

    }
}
