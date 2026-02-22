namespace Ctp0600P.Client.PLC.Common
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
        /// 风扇
        /// </summary>
        Fan = 1 << 2,

        /// <summary>
        /// 复位
        /// </summary>
        Reset = 1 << 8,

        /// <summary>
        /// 放行
        /// </summary>
        LetGo = 1 << 9,

        /// <summary>
        /// 初始化确认
        /// </summary>
        InitialAck = 1 << 10,
    }
}
