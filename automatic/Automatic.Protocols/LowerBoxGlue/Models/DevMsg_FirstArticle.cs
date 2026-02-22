using Automatic.Protocols.Common;
using System.Runtime.InteropServices;

namespace Automatic.Protocols.LowerBoxGlue.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class DevMsg_FirstArticle
    {
        public DevMsgFlag Flag;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 80)]
        public byte[] InspectData;
    }
}
