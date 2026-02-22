using Automatic.Protocols.Common;
using FutureTech.Protocols;
using System.Runtime.InteropServices;

namespace Automatic.Protocols.ModuleInBox.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class DevMsg_InBoxComplete
    {
        public const int ModuleInBoxDataSize = 200;
        public const int ModuleInBoxDataNum = 10;

        public DevMsgFlag Flag;

        [Endian(Endianness.BigEndian)]
        public ushort VectorCode;

        public String40 PackCode;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = ModuleInBoxDataSize * ModuleInBoxDataNum)]
        public byte[] ModuleInBoxDatas;
    }
}
