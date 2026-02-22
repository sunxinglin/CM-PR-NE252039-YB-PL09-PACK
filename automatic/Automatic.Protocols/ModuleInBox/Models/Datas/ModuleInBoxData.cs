using Automatic.Protocols.Common;
using FutureTech.Protocols;
using System.Runtime.InteropServices;

namespace Automatic.Protocols.ModuleInBox.Models.Datas
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class ModuleInBoxData
    {
        public String40 ModuleCode;

        [Endian(Endianness.BigEndian)]
        public uint PressurizeDuration;

        [Endian(Endianness.BigEndian)]
        public float ModuleLenth;

        [Endian(Endianness.BigEndian)]
        public float DownDistance;

        [Endian(Endianness.BigEndian)]
        public float DownPressure;

        [Endian(Endianness.BigEndian)]
        public float LeftPressure;

        [Endian(Endianness.BigEndian)]
        public float RightPressure;

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

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 122)]
        public byte[] Res;

        public string CompleteTime => $"{CompleteYear}-{CompleteMonth}-{CompleteDay} {CompleteHour}:{CompleteMinute}:{CompleteSecond}";
    }
}
