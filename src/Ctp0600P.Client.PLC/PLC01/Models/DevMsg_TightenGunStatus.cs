using Ctp0600P.Client.PLC.PLC01.Models.Flags;
using System.Runtime.InteropServices;

namespace Ctp0600P.Client.PLC.PLC01.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class DevMsg_TightenGunStatus
    {
        public TightenGunStatusFlag Flag;
    }
}
