using Automatic.Entity.DataDtos;
using Automatic.Protocols.Common;
using MediatR;

namespace Automatic.Protocols.TerminalReshape.Models.Wraps
{
    public class DealReqCompleteReshapeWraps : INotification
    {
        public int VectorCode { get; set; } = 1;
        public string PackCode { get; set; } = "";
        public string StartTime { get; set; } = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        public IList<PressureValue> PressureValue { get; set; }
    }

    public class DealReqCompleteGlueOkWraps
    {

    }

    public class DealReqCompleteGlueNgWraps : CommonErrWraps
    {

    }
}
