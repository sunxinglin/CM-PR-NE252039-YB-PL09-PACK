namespace Automatic.Protocols.Common
{
    public enum MstMsg_GeneralCmdFlags : ushort
    {
        None = 0,
        /// <summary>
        /// 心跳请求
        /// </summary>
        HeartBeatReq = 1 << 0,
        /// <summary>
        /// 心跳响应
        /// </summary>
        Heartbeat_Answer = 1 << 1,

        /// <summary>
        /// 急停请求
        /// </summary>
        EmgencyStop_Req = 1 << 2,

        /// <summary>
        /// 急停完成确认
        /// </summary>
        EmgencyStop_CompleteConfirm = 1 << 3,

        /// <summary>
        /// 恢复请求
        /// </summary>
        Recovery_Req = 1 << 4,

        /// <summary>
        /// 恢复完成确认
        /// </summary>
        Recovery_CompleteConfirm = 1 << 5,

        /// <summary>
        /// 初始化数据完成
        /// </summary>
        InitDataAckComplete = 1 << 15,

    }

    public class MstMsg_GeneralCmdFlagsBuilder : FlagsBuilder<MstMsg_GeneralCmdFlags>
    {
        public MstMsg_GeneralCmdFlagsBuilder(MstMsg_GeneralCmdFlags wCmd) : base(wCmd)
        {
        }

        public MstMsg_GeneralCmdFlagsBuilder SetHeartBeatReqOnOff(bool onoff)
        {
            SetOnOff(MstMsg_GeneralCmdFlags.HeartBeatReq, onoff);
            return this;
        }

        public MstMsg_GeneralCmdFlagsBuilder SetHeartBeatAnswerOnOff(bool onoff)
        {
            SetOnOff(MstMsg_GeneralCmdFlags.Heartbeat_Answer, onoff);
            return this;
        }

    }


}
