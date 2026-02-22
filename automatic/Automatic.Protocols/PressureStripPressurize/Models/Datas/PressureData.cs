using FutureTech.Protocols;

using System.Runtime.InteropServices;

namespace Automatic.Protocols.PressureStripPressurize.Models.Datas
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PressureData
    {
        [Endian(Endianness.BigEndian)]
        public uint KeepDuration;
        [Endian(Endianness.BigEndian)]
        public float PressureMax;
        [Endian(Endianness.BigEndian)]
        public float PressureAverage;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 20)]
        public byte[] Rees;
    }
}
