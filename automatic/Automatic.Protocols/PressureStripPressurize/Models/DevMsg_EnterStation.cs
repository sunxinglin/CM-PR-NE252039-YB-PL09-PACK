using Automatic.Protocols.Common;
using FutureTech.Protocols;
using System.Runtime.InteropServices;

namespace Automatic.Protocols.PressureStripPressurize.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class DevMsg_EnterStation
    {
        public DevMsgFlag Flag;

        [Endian(Endianness.BigEndian)]
        public ushort VectorCode;

        public String40 PackCode;
    }
}
