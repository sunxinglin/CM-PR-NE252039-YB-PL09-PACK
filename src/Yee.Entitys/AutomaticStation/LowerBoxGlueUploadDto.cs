using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yee.Entitys.AutomaticStation
{
    public class LowerBoxGlueUploadDto
    {
        public int VectorCode { get; set; }

        public string Pin { get; set; } = string.Empty;

        public string StepCode { get; set; } = string.Empty;

        public string StationCode { get; set; } = string.Empty;

        public Dictionary<string, object> Datas { get; set; } = null!;
    }
}
