using System.Runtime.InteropServices;

using Ctp0600P.Client.PLC.Common;

using FutureTech.Protocols;

namespace Ctp0600P.Client.PLC.PLC01.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class MstMsg_LeakStart
    {
        public RequestFlag Flag;

        //充气时长，分钟数
        [Endian(Endianness.BigEndian)]
        public ushort LeakDuration;

        //保压时长，分钟数
        [Endian(Endianness.BigEndian)]
        public ushort PressureDuration;

        //充气压力
        [Endian(Endianness.BigEndian)]
        public float LeakStress;

        //保压压力
        [Endian(Endianness.BigEndian)]
        public float PressureStress;

    }
}
