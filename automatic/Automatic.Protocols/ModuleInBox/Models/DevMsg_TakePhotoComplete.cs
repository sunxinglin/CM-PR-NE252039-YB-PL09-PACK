using Automatic.Protocols.Common;
using FutureTech.Protocols;
using System.Runtime.InteropServices;

namespace Automatic.Protocols.ModuleInBox.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class DevMsg_TakePhotoComplete
    {
        public DevMsgFlag Flag;

        public String40 CellCode;

        [Endian(Endianness.BigEndian)]
        public ushort Location;
    }
}
