using Ctp0600P.Client.PLC.Common;
using Ctp0600P.Client.PLC.PLC01.Models.Datas;
using FutureTech.Protocols;
using System.Runtime.InteropServices;

namespace Ctp0600P.Client.PLC.PLC01.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class DevMsg_TightenComplete
    {
        public RequestFlag Flag;

        [Endian(Endianness.BigEndian)]
        public ushort DeviceNo;

        [Endian(Endianness.BigEndian)]
        public ushort DeviceBrand;

        public TightenData TightenResult;

      
    }
}
