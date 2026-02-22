namespace Ctp0600P.Client.PLC.Common
{
    public enum DevMsg_GeneralCmdFlags : ushort
    {
        None = 0,
        /// <summary>
        /// PLC心跳请求
        /// </summary>
        HeartbeatReq = 1 << 0,

    }
}
