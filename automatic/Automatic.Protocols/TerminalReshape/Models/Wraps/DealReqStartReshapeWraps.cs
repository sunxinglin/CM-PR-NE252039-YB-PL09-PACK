using Automatic.Protocols.Common;

namespace Automatic.Protocols.TerminalReshape.Models.Wraps
{
    public class DealReqStartReshapeWraps
    {
        public int VectorCode { get; set; }

        public string PackCode { get; set; }
    }

    public class DealReqStartGlueOkWraps
    {
        public ushort ShapeLevel { get; set; }
    }

    public class DealReqStartGlueNgWraps : CommonErrWraps
    {

    }
}
