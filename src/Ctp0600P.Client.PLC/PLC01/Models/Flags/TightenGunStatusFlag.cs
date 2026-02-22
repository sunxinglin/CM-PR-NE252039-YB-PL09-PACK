namespace Ctp0600P.Client.PLC.PLC01.Models.Flags
{
    public enum TightenGunStatusFlag : ushort
    {
        None = 0,
        /// <summary>
        /// 拧紧枪就绪
        /// </summary>
        Ready = 1 << 0,
        /// <summary>
        /// 拧紧枪使能
        /// </summary>
        Enabled = 1 << 1,
        /// <summary>
        /// 拧紧中
        /// </summary>
        Tightening = 1 << 2,
        /// <summary>
        /// 拧紧枪报警
        /// </summary>
        Alarm = 1 << 3,
    }
}
