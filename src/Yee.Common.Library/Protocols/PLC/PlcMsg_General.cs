using System.Runtime.InteropServices;

namespace Yee.Common.Library.Protocols
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PlcMsg_General
    {
        /// <summary>
        /// 通用命令字
        /// </summary>
        public PlcFlags_GeneralCmdWord GeneralCmdWord;
        /// <summary>
        /// 通用状态字
        /// </summary>
        public PlcFlags_GeneralStatus GeneralStatus;

        /// <summary>
        /// 预留N个字节
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 6)]
        public byte[] Reserved;
    }
}
