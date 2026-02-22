using Automatic.Protocols.Common;

namespace Automatic.Protocols.TerminalReshape.Models.Wraps
{
    public class DealReqVectorEnterWraps
    {
        public int VectorCode { get; set; }

        public string PackCode { get; set; }
    }

    public class DealReqVectorEnterOkWraps
    {
        public ushort ShapeLevel { get; set; } = 0;
        public uint LeftTime { get; set; } = 0;
    }

    public class DealReqVectorEnterNgWraps : CommonErrWraps
    {

    }
}
