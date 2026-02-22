namespace Ctp0600P.Client.PLC.Common
{
    public enum TightenStartNGFlag : ushort
    {
        None = 0,
        /// <summary>
        /// 上使能失败
        /// </summary>
        EnableFail = 1 << 0,
    }

    public class TightenStartNGFlagBuilder : FlagsBuilder<TightenStartNGFlag>
    {
        public TightenStartNGFlagBuilder(TightenStartNGFlag wCmd) : base(wCmd)
        {

        }

        public TightenStartNGFlagBuilder Ack(TightenStartNGFlag flag)
        {
            SetOnOff(TightenStartNGFlag.EnableFail, flag.HasFlag(TightenStartNGFlag.EnableFail))
                .Build();
            return this;
        }

        public TightenStartNGFlagBuilder Reset()
        {
            SetOnOff(TightenStartNGFlag.EnableFail, false)
                .Build();
            return this;
        }
    }
}
