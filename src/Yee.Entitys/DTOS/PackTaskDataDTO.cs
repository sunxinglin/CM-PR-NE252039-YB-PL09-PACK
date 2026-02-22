using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yee.Entitys.DTOS
{
    public class PackTaskRecordDataDTO
    {
        public string PackCode { get; set; }
        public string StationCode { get; set; }
        public string Status { get; set; }
        public string CreateTime { get; set; }
    }

    public class PackTaskMainDataDTO
    {
        public int TotalCount { get; set; }
        public List<PackTaskRecordDataDTO> Items { get; set; }  
    }
}
