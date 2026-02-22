using FutureTech.Protocols;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Automatic.Protocols.TerminalReshape.Models.Datas
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class ReshapeData
    {
        [Endian(Endianness.BigEndian)]
        public uint KeepDuration;
        [Endian(Endianness.BigEndian)]
        public float PressureMax;
        [Endian(Endianness.BigEndian)]
        public float PressureAverage;
        [Endian(Endianness.BigEndian)]
        public float ShoudlerHeight;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 20)]
        public byte[] Rees;
    }
}
