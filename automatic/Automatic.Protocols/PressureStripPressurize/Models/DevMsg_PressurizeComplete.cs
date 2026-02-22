using Automatic.Protocols.Common;
using FutureTech.Protocols;
using System.Runtime.InteropServices;

namespace Automatic.Protocols.PressureStripPressurize.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class DevMsg_PressurizeComplete
    {
        public DevMsgFlag Flag;

        [Endian(Endianness.BigEndian)]
        public ushort VectorCode;

        public String40 PackCode;

        [Endian(Endianness.BigEndian)]
        public ushort CompleteYear;

        [Endian(Endianness.BigEndian)]
        public ushort CompleteMonth;

        [Endian(Endianness.BigEndian)]
        public ushort CompleteDay;

        [Endian(Endianness.BigEndian)]
        public ushort CompleteHour;

        [Endian(Endianness.BigEndian)]
        public ushort CompleteMinute;

        [Endian(Endianness.BigEndian)]
        public ushort CompleteSecond;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 640)]
        public byte[] PressureDatas;

        public string CompleteTime => $"{CompleteYear}/{CompleteMonth}/{CompleteDay} {CompleteHour}:{CompleteMinute}:{CompleteSecond}";
    }
}
