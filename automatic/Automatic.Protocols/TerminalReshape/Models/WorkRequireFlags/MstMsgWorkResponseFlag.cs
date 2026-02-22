using Automatic.Protocols.Common;

namespace Automatic.Protocols.TerminalReshape.Models.WorkRequireFlags
{
    public enum MstMsgAckVectorEnterFlag : ushort
    {
        None = 0,
        AckVectorEnter = 1 << 0,
        VectorEnterOK = 1 << 1,
        VectorEnterNG = 1 << 2,
    }
    public enum MstMsgAckStartReshapeFlag : ushort
    {
        None = 0,
        AckStartReshape = 1 << 0,
        StartReshapeOK = 1 << 1,
        StartReshapeNG = 1 << 2,
    }

    public enum MstMsgAckComplateReshapeFlag : ushort
    {
        None = 0,
        AckCompleteReshape = 1 << 0,
        CompleteReshapeOK = 1 << 1,
        CompleteReshapeNG = 1 << 2,
    }

    public class MstMsgAckVectorEnterFlagBuilder : FlagsBuilder<MstMsgAckVectorEnterFlag>
    {
        public MstMsgAckVectorEnterFlagBuilder(MstMsgAckVectorEnterFlag wCmd) : base(wCmd)
        {

        }
        public MstMsgAckVectorEnterFlagBuilder AckVectorEnterReq(bool isOk)
        {
            SetOnOff(MstMsgAckVectorEnterFlag.AckVectorEnter, true)
                .SetOnOff(MstMsgAckVectorEnterFlag.VectorEnterOK, isOk)
                .SetOnOff(MstMsgAckVectorEnterFlag.VectorEnterNG, !isOk)
                .Build();
            return this;
        }

        public MstMsgAckVectorEnterFlagBuilder RestVectorEnterReq()
        {
            SetOnOff(MstMsgAckVectorEnterFlag.AckVectorEnter, false)
                .SetOnOff(MstMsgAckVectorEnterFlag.VectorEnterOK, false)
                .SetOnOff(MstMsgAckVectorEnterFlag.VectorEnterNG, false)
                .Build();
            return this;
        }
    }

    public class MstMsgAckStartReshapeFlagBuilder : FlagsBuilder<MstMsgAckStartReshapeFlag>
    {
        public MstMsgAckStartReshapeFlagBuilder(MstMsgAckStartReshapeFlag wCmd) : base(wCmd)
        {

        }
        public MstMsgAckStartReshapeFlagBuilder AckStartReshape(bool isOk)
        {
            SetOnOff(MstMsgAckStartReshapeFlag.AckStartReshape, true)
                .SetOnOff(MstMsgAckStartReshapeFlag.StartReshapeOK, isOk)
                .SetOnOff(MstMsgAckStartReshapeFlag.StartReshapeNG, !isOk)
                .Build();
            return this;
        }

        public MstMsgAckStartReshapeFlagBuilder ResetStartReshape()
        {
            SetOnOff(MstMsgAckStartReshapeFlag.AckStartReshape, false)
                .SetOnOff(MstMsgAckStartReshapeFlag.StartReshapeOK, false)
                .SetOnOff(MstMsgAckStartReshapeFlag.StartReshapeNG, false)
                .Build();
            return this;
        }
    }

    public class MstMsgAckComplateReshapeFlagBuilder : FlagsBuilder<MstMsgAckComplateReshapeFlag>
    {
        public MstMsgAckComplateReshapeFlagBuilder(MstMsgAckComplateReshapeFlag wCmd) : base(wCmd)
        {

        }
        public MstMsgAckComplateReshapeFlagBuilder AckCompleteReshape(bool isOk)
        {
            SetOnOff(MstMsgAckComplateReshapeFlag.AckCompleteReshape, true)
                .SetOnOff(MstMsgAckComplateReshapeFlag.CompleteReshapeOK, isOk)
                .SetOnOff(MstMsgAckComplateReshapeFlag.CompleteReshapeNG, !isOk)
                .Build();
            return this;
        }

        public MstMsgAckComplateReshapeFlagBuilder ResetCompleteReshape()
        {
            SetOnOff(MstMsgAckComplateReshapeFlag.AckCompleteReshape, false)
                .SetOnOff(MstMsgAckComplateReshapeFlag.CompleteReshapeOK, false)
                .SetOnOff(MstMsgAckComplateReshapeFlag.CompleteReshapeNG, false)
                .Build();
            return this;
        }
    }
  
}
