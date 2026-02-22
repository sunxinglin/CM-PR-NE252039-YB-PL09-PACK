using Automatic.Protocols.Common;
using FutureTech.Protocols;
using System.Runtime.InteropServices;

namespace Automatic.Protocols.PressureStripPressurize.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class MstMsg_EnterStation
    {
        public MstMsgFlag Flag;

        [Endian(Endianness.BigEndian)]
        public ushort ErrorCode;

        [Endian(Endianness.BigEndian)]
        public uint GlueRemainDuration;
    }
}
