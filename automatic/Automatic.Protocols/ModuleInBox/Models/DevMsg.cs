using Automatic.Protocols.Common;
using System.Runtime.InteropServices;

namespace Automatic.Protocols.ModuleInBox.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class DevMsg
    {
        /// <summary>
        /// 通用命令
        /// </summary>
        public DevMsg_GeneralCmdFlags CmdFlags;

        /// <summary>
        /// 通用状态
        /// </summary>
        public DevMsg_GeneralStatus Status;

        /// <summary>
        /// 预留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 10)]
        public byte[] Res0;

        /// <summary>
        /// 拍电芯码完成
        /// </summary>
        public DevMsg_TakePhotoComplete TakePhotoComplete;

        /// <summary>
        /// 预留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        public byte[] Res1;

        /// <summary>
        /// 进工位
        /// </summary>
        public DevMsg_EnterStation EnterStation;

        /// <summary>
        /// 预留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        public byte[] Res3;

        /// <summary>
        /// 开始工作
        /// </summary>
        public DevMsg_StartInBox StartInBox;

        /// <summary>
        /// 预留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        public byte[] Res4;

        /// <summary>
        /// 单个入箱完成
        /// </summary>
        public DevMsg_SingleInBoxComplete SingleInBoxComplete;

        /// <summary>
        /// 预留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        public byte[] Res2;

        /// <summary>
        /// 全部入箱完成
        /// </summary>
        public DevMsg_InBoxComplete InBoxComplete;

    }
}
