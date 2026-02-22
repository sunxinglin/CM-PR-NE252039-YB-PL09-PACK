namespace Automatic.Protocols.Common
{
    public enum DevMsg_GeneralStatus : ushort
    {
        None = 0,
        /// <summary>
        /// 自动模式
        /// </summary>

        Auto = 1 << 0,

        /// <summary>
        /// 手动模式
        /// </summary>

        Manual = 1 << 1,
        /// <summary>
        /// 急停
        /// </summary>

        EmgencyStop = 1 << 2,

        /// <summary>
        /// 未回零
        /// </summary>

        NotYetHoming = 1 << 3,
        /// <summary>
        /// 回零中
        /// </summary>

        Homing = 1 << 4,
        /// <summary>
        /// 回零完成
        /// </summary>

        HomeCompleted = 1 << 5,

        /// <summary>
        /// 空闲——等待执行任务
        /// </summary>

        Idle = 1 << 6,
        /// <summary>
        /// 繁忙
        /// </summary>

        Busy = 1 << 7,

        /// <summary>
        /// 
        /// </summary>

        aaaaaaaaaa = 1 << 15,
    }
}
