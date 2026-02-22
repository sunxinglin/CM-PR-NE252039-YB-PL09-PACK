using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic.Protocols.Common
{
    public enum MstMsgFlag : ushort
    {
        None = 0,
        Ack = 1 << 0,
        OK = 1 << 1,
        NG = 1 << 2,
    }

    public class MstMsgFlagBuilder : FlagsBuilder<MstMsgFlag>
    {
        public MstMsgFlagBuilder(MstMsgFlag wCmd) : base(wCmd)
        {

        }

        public MstMsgFlagBuilder Ack(bool isOk)
        {
            SetOnOff(MstMsgFlag.Ack, true)
                .SetOnOff(MstMsgFlag.OK, isOk)
                .SetOnOff(MstMsgFlag.NG, !isOk)
                .Build();
            return this;
        }

        public MstMsgFlagBuilder Reset()
        {
            SetOnOff(MstMsgFlag.Ack, false)
                .SetOnOff(MstMsgFlag.OK, false)
                .SetOnOff(MstMsgFlag.NG, false)
                .Build();
            return this;
        }
    }
}
