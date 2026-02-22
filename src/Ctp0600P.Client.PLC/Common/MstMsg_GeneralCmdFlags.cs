namespace Ctp0600P.Client.PLC.Common
{
    public enum MstMsg_GeneralCmdFlags : ushort
    {
        None = 0,
        /// <summary>
        /// 心跳响应
        /// </summary>
        HeartBeatAnswer = 1 << 0,

    }

    public class MstMsg_GeneralCmdFlagsBuilder : FlagsBuilder<MstMsg_GeneralCmdFlags>
    {
        public MstMsg_GeneralCmdFlagsBuilder(MstMsg_GeneralCmdFlags wCmd) : base(wCmd)
        {
        }

        public MstMsg_GeneralCmdFlagsBuilder SetHeartBeatAnswerOnOff(bool onoff)
        {
            SetOnOff(MstMsg_GeneralCmdFlags.HeartBeatAnswer, onoff);
            return this;
        }

    }


}
