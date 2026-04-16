using System.Runtime.InteropServices;

using Ctp0600P.Client.PLC.Common;

namespace Ctp0600P.Client.PLC.PLC01.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class MstMsg_LeakComplete
    {
        public ResponseFlag Flag;
    }
}
