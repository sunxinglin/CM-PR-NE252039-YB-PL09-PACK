using Automatic.Protocols.Common;
using FutureTech.Protocols;
using System.Runtime.InteropServices;

namespace Automatic.Protocols.LowerBoxGlue.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class DevMsg_ReGlueComplete
    {
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

        public string ReGlueStartTime => $"{StartYear}/{StartMonth}/{StartDay} {StartHour}:{StartMinute}:{StartSecond}";

    }
}
