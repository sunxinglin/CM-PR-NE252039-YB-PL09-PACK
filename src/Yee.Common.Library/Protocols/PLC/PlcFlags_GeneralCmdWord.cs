namespace Yee.Common.Library.Protocols
{
    /// <summary>
    /// 通用的控制字
    /// </summary>
    [Flags]
    public enum PlcFlags_GeneralCmdWord : ushort
    {
        None = 0,
        /// <summary>
        /// 心跳恢复
        /// </summary>
        HeartBeatAck = 1 << 0,
        /// <summary>
        /// 心跳请求
        /// </summary>
        HeartBeatReq = 1 << 1,

        /// <summary>
        /// 急停确认
        /// </summary>
        EmgencyStopAck = 1 << 2,
        /// <summary>
        /// 急停完成
        /// </summary>
        EmgencyStopFin = 1 << 3,
        /// <summary>
        /// 恢复确认
        /// </summary>
        ResumeAck = 1 << 4,
        /// <summary>
        /// 恢复完成
        /// </summary>
        ResumeFin = 1 << 5,


        /// <summary>
        /// 自动模式切换：确认。只有在自动模式下，PLC才会接收上位机的作业指令。
        /// </summary>
        AutoModeAck = 1 << 6,
        /// <summary>
        /// 自动模式切换：完成
        /// </summary>
        AutoModeFin = 1 << 7,

        /// <summary>
        /// 手动模式切换：请求。当进入手动模式后，PLC不会接收上位机的作业指令。
        /// </summary>
        ManualModeAck = 1 << 8,
        /// <summary>
        /// 手动模式切换：完成确认
        /// </summary>
        ManualModeFin = 1 << 9,

        /// <summary>
        /// 回归原点：请求。
        /// </summary>
        HomeAck = 1 << 10,
        /// <summary>
        /// 回归原点：完成确认。
        /// </summary>
        HomeFin = 1 << 11,


        /// <summary>
        /// 初始化数据请求
        /// </summary>
        InitDataReq = 1 << 15,
    }
}
