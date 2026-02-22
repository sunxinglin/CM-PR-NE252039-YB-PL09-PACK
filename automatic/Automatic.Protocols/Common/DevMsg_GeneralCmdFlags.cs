namespace Automatic.Protocols.Common
{
    public enum DevMsg_GeneralCmdFlags : ushort
    {
        None = 0,
        /// <summary>
        /// 心跳响应
        /// </summary>

        Heartbeat_Answer = 1 << 0,
        /// <summary>
        /// 心跳请求
        /// </summary>

        Heartbeat_Req = 1 << 1,

        /// <summary>
        /// 急停确认
        /// </summary>

        EmgencyStop_Confirmed = 1 << 2,

        /// <summary>
        /// 急停完成
        /// </summary>

        EmgencyStop_Finish = 1 << 3,

        /// <summary>
        /// 恢复确认
        /// </summary>

        Recovery_Confirm = 1 << 4,

        /// <summary>
        /// 恢复完成
        /// </summary>

        Recovery_Complete = 1 << 5,


        /// <summary>
        /// 请求初始化数据
        /// </summary>

        Req_Init_Data = 1 << 15,

    }
}
