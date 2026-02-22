using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic.Entity.DataDtos
{
    public class LowerBoxReGlueDataDto
    {
        public string PackCode { get; set; } = string.Empty;

        public int VectorCode { get; set; }

        public string StationCode { get; set; } = string.Empty;

        public string GlueStartTime { get; set; } = string.Empty;
    }
}
