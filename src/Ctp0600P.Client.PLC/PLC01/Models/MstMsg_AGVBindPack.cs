using Ctp0600P.Client.PLC.Common;
using FutureTech.Protocols;
using System.Runtime.InteropServices;

namespace Ctp0600P.Client.PLC.PLC01.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class MstMsg_AGVBindPack
    {
        public RequestFlag Flag;

        [Endian(Endianness.BigEndian)]
        public ushort Behavior;

        [Endian(Endianness.BigEndian)]
        public ushort AGVNo;

        public String40 PackCode;

        public String40 HolderBarcode;
        
        public String40 StationCode;
    }
}
