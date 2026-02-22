using Automatic.Protocols.Common;
using FutureTech.Protocols;
using System.Runtime.InteropServices;

namespace Automatic.Protocols.LowerBoxGlue.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class DevMsg_GlueComplete
    {
        public const int GlueDataSize = 16;
        public const int GlueDataCount = 30;

        public DevMsgFlag Flag;

        [Endian(Endianness.BigEndian)]
        public ushort VectorCode;

        public String40 PackCode;

        [Endian(Endianness.BigEndian)]
        public ushort StartYear;

        [Endian(Endianness.BigEndian)]
        public ushort StartMonth;

        [Endian(Endianness.BigEndian)]
        public ushort StartDay;

        [Endian(Endianness.BigEndian)]
        public ushort StartHour;

        [Endian(Endianness.BigEndian)]
        public ushort StartMinute;

        [Endian(Endianness.BigEndian)]
        public ushort StartSecond;

        [Endian(Endianness.BigEndian)]
        public float GlueTotalWeight;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = GlueDataCount * GlueDataSize)]
        public byte[] GlueDatas;

        public string GlueStartTime => $"{StartYear}/{StartMonth}/{StartDay} {StartHour}:{StartMinute}:{StartSecond}";

    }
}
