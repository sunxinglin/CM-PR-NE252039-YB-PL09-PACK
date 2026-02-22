using Automatic.Protocols.LowerBoxGlue.Models.Datas;
using MediatR;

namespace Automatic.Protocols.LowerBoxGlue.Models.Wraps
{
    public class DealReqGlueCompleteWraps : INotification
    {
        public int VectorCode { get; set; } = 1;
        public string PackCode { get; set; } = "";
        public string StartTime { get; set; } = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        public Dictionary<string, object> GlueDatas { get; set; }
    }
}
