namespace Automatic.Protocols.TerminalReshape.Models.WorkRequireFlags
{
    public enum DevMsgReqVectorEnterFlag : ushort
    {
        None = 0,
        ReqVectorEnter = 1 << 0,
    }

    public enum DevMsgReqStartReshapeFlag : ushort
    {
        None = 0,
        ReqStartReshape = 1 << 0,
    }

    public enum DevMsgReqComplateReshapeFlag : ushort
    {
        None = 0,
        ReqCompleteReshape = 1 << 0,
    }
}
