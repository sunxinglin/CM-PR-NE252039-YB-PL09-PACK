using Automatic.Protocols.ModuleInBox.Models.Datas;

namespace Automatic.Protocols.ModuleInBox.Models.Wraps
{
    public class DealReqInBoxCompleteWraps
    {
        public int VectorCode { get; set; }

        public string PackCode { get; set; }

        public IList<ModuleInBoxData> ModuleInBoxDatas { get; set; }
    }
}
