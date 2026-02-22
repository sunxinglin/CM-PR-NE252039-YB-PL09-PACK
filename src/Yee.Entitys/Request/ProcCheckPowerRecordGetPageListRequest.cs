using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yee.Entitys.Request
{
    public class ProcCheckPowerRecordGetPageListRequest : PageBaseRequest
    {
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ModuleName { get; set; }
        public string? StationCode { get; set; }
        public string? PackCode { get; set; }
    }
}
