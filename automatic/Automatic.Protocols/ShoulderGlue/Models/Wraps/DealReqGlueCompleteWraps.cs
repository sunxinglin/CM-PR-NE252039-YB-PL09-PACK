using Automatic.Protocols.ShoulderGlue.Models.Datas;
using MediatR;

namespace Automatic.Protocols.ShoulderGlue.Models.Wraps
{
    public class DealReqGlueCompleteWraps : INotification
    {
        public int VectorCode { get; set; } = 1;
        public string PackCode { get; set; } = "";
        public string StartTime { get; set; } = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        public IList<GlueData> GlueDatas { get; set; }
    }
}
