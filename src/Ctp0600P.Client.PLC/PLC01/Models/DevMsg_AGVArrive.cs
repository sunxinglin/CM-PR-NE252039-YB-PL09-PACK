using System.Runtime.InteropServices;

using Ctp0600P.Client.PLC.Common;

using FutureTech.Protocols;

namespace Ctp0600P.Client.PLC.PLC01.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class DevMsg_AGVArrive
    {
        public RequestFlag Flag;

        [Endian(Endianness.BigEndian)]
        public ushort AGVNo;

        public String40 PackCode;

        public String40 StationCode;

    }
}
