using Automatic.Protocols.Common;
using FutureTech.Protocols;
using System.Runtime.InteropServices;

namespace Automatic.Protocols.HeatingFilmPressurize.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class MstMsg_EnterStation
    {
        public MstMsgFlag Flag;

        [Endian(Endianness.BigEndian)]
        public ushort ErrorCode;

    }
}
