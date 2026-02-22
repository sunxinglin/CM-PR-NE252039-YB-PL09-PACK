using FutureTech.Protocols;
using System.Runtime.InteropServices;

namespace Automatic.Protocols.HeatingFilmPressurize.Models.Datas
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PressureData
    {
        [Endian(Endianness.BigEndian)]
        public int KeepDuration;

        [Endian(Endianness.BigEndian)]
        public float PressureMax;

        [Endian(Endianness.BigEndian)]
        public float PressureAverage;

        /// <summary>
        /// 预留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 20)]
        public byte[] Rees;
    }
}
