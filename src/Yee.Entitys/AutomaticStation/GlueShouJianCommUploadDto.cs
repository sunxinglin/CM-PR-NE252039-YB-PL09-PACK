using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yee.Entitys.AutomaticStation
{
    public class GlueShouJianCommUploadDto
    {
        public int VectorCode { get; set; }

        public string DateTime { get; set; } = string.Empty;

        public string StepCode { get; set; } = string.Empty;
        public GlueShouJianCommUploadType ShouJianCommUploadType { get; set; }

        public string StationCode { get; set; } = string.Empty;

        public Dictionary<string, object> Datas { get; set; } = null!;
    }

    public enum GlueShouJianCommUploadType
    {
        BeamGule=1,
        LowerBoxGlue=2,
        ShoulderGlue=3
    }
}
