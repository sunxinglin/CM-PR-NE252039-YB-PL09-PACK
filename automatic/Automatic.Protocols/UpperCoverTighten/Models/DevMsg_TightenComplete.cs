using Automatic.Protocols.Common;
using FutureTech.Protocols;
using System.Runtime.InteropServices;

namespace Automatic.Protocols.UpperCoverTighten.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class DevMsg_TightenComplete
    {
        public const int TightenNum = 150;
        public const int TightenStructSize = 52;

        public DevMsgFlag Flag;

        [Endian(Endianness.BigEndian)]
        public ushort VectorCode;

        public String40 PackCode;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = TightenStructSize * TightenNum)]
        public byte[] TightenDatas;
    }
}
