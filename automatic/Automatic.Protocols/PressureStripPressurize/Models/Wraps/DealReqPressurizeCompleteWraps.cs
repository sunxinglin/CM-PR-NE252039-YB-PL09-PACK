using Automatic.Entity.DataDtos;
using Automatic.Protocols.Common;
using MediatR;

namespace Automatic.Protocols.PressureStripPressurize.Models.Wraps
{
    public class DealReqPressurizeCompleteWraps :INotification
    {
        public int AgvCode { get; set; }
        public string PackCode { get; set; } = "";
        public string CompleteTime { get; set; } = "";
        public IList<PressureValue> PressureValue { get; set; }
    }
}
