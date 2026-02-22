using Ctp0600P.Client.PLC.Common;
using System.Runtime.InteropServices;

namespace Ctp0600P.Client.PLC.PLC01.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class DevMsg_AGVBindPack
    {
        public ResponseFlag Flag;
    }
}
