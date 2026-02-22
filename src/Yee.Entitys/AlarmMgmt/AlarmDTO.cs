using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.Common;
using Yee.Entitys.Production;

namespace Yee.Entitys.DTOS
{
    public class AlarmDTO
    {
        public string? DeviceNo { get; set; }
        public string? StepCode { get; set; }
        public string? StationCode { get; set; }
        public string? PackCode { get; set; }
        public AlarmCode Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime OccurTime { get; set; }
        public bool IsFinish { get; set; } = false;
    }

    public enum AlarmAction
    {
        Occur = 0,
        Clear = 1
    }

}
