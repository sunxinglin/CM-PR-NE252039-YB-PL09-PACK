using Automatic.Protocols.Common;
using System.Runtime.InteropServices;

namespace Automatic.Protocols.PressureStripPressurize.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class MstMsg
    {
        /// <summary>
        /// 通用命令
        /// </summary>
        public MstMsg_GeneralCmdFlags CmdFlags;

        /// <summary>
        /// 预留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 12)]
        public byte[] Res0;

        /// <summary>
        /// 进工位
        /// </summary>
        public MstMsg_EnterStation EnterStation;

        /// <summary>
        /// 预留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 96)]
        public byte[] Res1;

        /// <summary>
        /// 开始加压
        /// </summary>
        public MstMsg_StartPressurize StartPressurize;

        /// <summary>
        /// 预留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        public byte[] Res2;

        /// <summary>
        /// 加压完成
        /// </summary>
        public MstMsg_PressurizeComplete PressurizeComplete;
    }
}
