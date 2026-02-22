using Automatic.Protocols.Common;
using System.Runtime.InteropServices;

namespace Automatic.Protocols.ShoulderGlue.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class DevMsg_FirstArticle
    {
        public DevMsgFlag Flag;
    }
}
