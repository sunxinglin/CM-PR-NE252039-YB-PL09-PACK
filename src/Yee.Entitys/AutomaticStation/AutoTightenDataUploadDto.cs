using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yee.Entitys.DBEntity;

namespace Yee.Entitys.AutomaticStation
{
    public class AutoTightenDataUploadDto
    {
        public string Pin { get; set; } = string.Empty;

        public int VectorCode { get; set; }

        public string StepCode { get; set; } = string.Empty;

        public string StationCode { get; set; } = string.Empty;

        public string BoltType {  get; set; } = string.Empty;

        public IList<AutoBlotInfo> TightenDatas { get; set; } = null!;
    }
}
