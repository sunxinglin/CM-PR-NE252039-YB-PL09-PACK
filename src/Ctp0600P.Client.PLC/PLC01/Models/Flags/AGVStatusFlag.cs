namespace Ctp0600P.Client.PLC.PLC01.Models.Flags
{
    public enum AGVStatusFlag : ushort
    {
        None = 0,
        /// <summary>
        /// AGV允许进入
        /// </summary>
        AGVAllowEntry = 1 << 0,
        /// <summary>
        /// AGV允许离开
        /// </summary>
        AGVAllowLeave = 1 << 1,
        /// <summary>
        /// AGV在位
        /// </summary>
        AGVInStation = 1 << 2,
        /// <summary>
        /// AGV正在进入
        /// </summary>
        AGVEntrying = 1 << 3,
        /// <summary>
        /// AGV正在离开
        /// </summary>
        AGVLeaving = 1 << 4,
        /// <summary>
        /// AGV请求写码
        /// </summary>
        AGVReqWriteBarcode = 1 << 5,
        /// <summary>
        /// AGV写码完成
        /// </summary>
        AGVWriteBarcodeComplete = 1 << 6,
    }
}
