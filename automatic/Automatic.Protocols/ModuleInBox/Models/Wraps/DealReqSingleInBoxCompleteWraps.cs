using Automatic.Protocols.ModuleInBox.Models.Datas;

namespace Automatic.Protocols.ModuleInBox.Models.Wraps
{
    public class DealReqSingleInBoxCompleteWraps
    {
        public int VectorCode { get; set; }

        public string PackCode { get; set; }

        public string ModuleCode { get; set; }

        public int ModuleLocation { get; set; }
    }
}
