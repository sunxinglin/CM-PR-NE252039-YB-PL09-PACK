namespace Ctp0600P.Client.PLC.Common
{
    public enum MstMsg_GeneralStatus : ushort
    {
        None = 0,

        /// <summary>
        /// 报警
        /// </summary>
        Alarm = 1 << 0,

        /// <summary>
        /// 可放行
        /// </summary>
        LetGo = 1 << 1,
        
        /// <summary>
        /// 超时报警
        /// </summary>
        OverrunAlarm = 1 << 2,

        /// <summary>
        /// 复位确认
        /// </summary>
        ResetConfirm = 1 << 8,

        /// <summary>
        /// 放行确认
        /// </summary>
        LetGoConfirm = 1 << 9,

        /// <summary>
        /// 初始化请求
        /// </summary>
        InitialReq = 1 << 10,

    }

    public class MstMsg_GeneralStatusBuilder : FlagsBuilder<MstMsg_GeneralStatus>
    {
        public MstMsg_GeneralStatusBuilder(MstMsg_GeneralStatus wCmd) : base(wCmd)
        {
        }

        public MstMsg_GeneralStatusBuilder SetAlarmOnOff(bool onoff)
        {
            SetOnOff(MstMsg_GeneralStatus.Alarm, onoff);
            return this;
        }

        public MstMsg_GeneralStatusBuilder SetLetGoOnOff(bool onoff)
        {
            SetOnOff(MstMsg_GeneralStatus.LetGo, onoff);
            return this;
        }

        public MstMsg_GeneralStatusBuilder SetOverrunAlarmOnOff(bool onoff)
        {
            SetOnOff(MstMsg_GeneralStatus.OverrunAlarm, onoff);
            return this;
        }

        public MstMsg_GeneralStatusBuilder SetResetComfirmOnOff(bool onoff)
        {
            SetOnOff(MstMsg_GeneralStatus.ResetConfirm, onoff);
            return this;
        }

        public MstMsg_GeneralStatusBuilder SetLetGoConfirmOnOff(bool onoff)
        {
            SetOnOff(MstMsg_GeneralStatus.LetGoConfirm, onoff);
            return this;
        }

        public MstMsg_GeneralStatusBuilder SetInitialReqOnOff(bool onoff)
        {
            SetOnOff(MstMsg_GeneralStatus.InitialReq, onoff);
            return this;
        }
    }
}
