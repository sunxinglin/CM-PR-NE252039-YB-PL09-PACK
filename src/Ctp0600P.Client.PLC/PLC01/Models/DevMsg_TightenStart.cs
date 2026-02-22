using Ctp0600P.Client.PLC.Common;
using FutureTech.Protocols;
using System.Runtime.InteropServices;

namespace Ctp0600P.Client.PLC.PLC01.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class DevMsg_TightenStart
    {
        public ResponseFlag Flag;

        public TightenStartNGFlag NGFlag;

       
    }
}
