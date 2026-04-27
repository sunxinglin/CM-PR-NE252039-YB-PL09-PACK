using System.Runtime.InteropServices;

using Ctp0600P.Client.PLC.Common;

using FutureTech.Protocols;

namespace Ctp0600P.Client.PLC.PLC01.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class DevMsg_LeakComplete
    {
        public RequestFlag Flag;

        [Endian(Endianness.BigEndian)]
        public ushort LeakStartTime_Year;

        [Endian(Endianness.BigEndian)]
        public ushort LeakStartTime_Month;

        [Endian(Endianness.BigEndian)]
        public ushort LeakStartTime_Day;

        [Endian(Endianness.BigEndian)]
        public ushort LeakStartTime_Hour;

        [Endian(Endianness.BigEndian)]
        public ushort LeakStartTime_Minute;

        [Endian(Endianness.BigEndian)]
        public ushort LeakStartTime_Second;


        [Endian(Endianness.BigEndian)]
        public ushort LeakCompleteTime_Year;

        [Endian(Endianness.BigEndian)]
        public ushort LeakCompleteTime_Month;

        [Endian(Endianness.BigEndian)]
        public ushort LeakCompleteTime_Day;

        [Endian(Endianness.BigEndian)]
        public ushort LeakCompleteTime_Hour;

        [Endian(Endianness.BigEndian)]
        public ushort LeakCompleteTime_Minute;

        [Endian(Endianness.BigEndian)]
        public ushort LeakCompleteTime_Second;


        [Endian(Endianness.BigEndian)]
        public float LeakTotalPower;

        [Endian(Endianness.BigEndian)]
        public float LeakProportionalPressure;

        [Endian(Endianness.BigEndian)]
        public float LeakAdkerPressure;

        [Endian(Endianness.BigEndian)]
        public ushort LeakTime;

        [Endian(Endianness.BigEndian)]
        public ushort LeakKeepTime;

        [Endian(Endianness.BigEndian)]
        public float LeakPressPower;

        [Endian(Endianness.BigEndian)]
        public float LeakKeepPressPower;

        [Endian(Endianness.BigEndian)]
        public ushort LeakRealTime;

        public string LeakStartTime => TryParseDateTime(LeakStartTime_Year, LeakStartTime_Month, LeakStartTime_Day, LeakStartTime_Hour, LeakStartTime_Minute, LeakStartTime_Second);
        public string LeakCompleteTime => TryParseDateTime(LeakCompleteTime_Year, LeakCompleteTime_Month, LeakCompleteTime_Day, LeakCompleteTime_Hour, LeakCompleteTime_Minute, LeakCompleteTime_Second);

        private static string TryParseDateTime(ushort year, ushort month, ushort day, ushort hour, ushort minute, ushort second)
        {
            if (year == 0 || month == 0 || day == 0)
                return string.Empty;
            try
            {
                return new DateTime(year, month, day, hour, minute, second).ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
