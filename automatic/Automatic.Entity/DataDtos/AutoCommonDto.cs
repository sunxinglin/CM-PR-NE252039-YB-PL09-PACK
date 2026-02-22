using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic.Entity.DataDtos
{
    public class AutoCommonDto
    {
    }

    public class CreateMessionRecordDto
    {
        public string Pin { get; set; } = string.Empty;
        public string ProductPn { get; set; } = "";
        public string StationCode { get; set; } = string.Empty;
        public string MessionType { get; set; } = string.Empty;
        public int VectorCode { get; set; }
    }
}
