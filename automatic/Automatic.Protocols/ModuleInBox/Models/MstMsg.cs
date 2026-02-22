using Automatic.Protocols.Common;
using System.Runtime.InteropServices;

namespace Automatic.Protocols.ModuleInBox.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class MstMsg
    {
        /// <summary>
        /// 通用状态
        /// </summary>
        public MstMsg_GeneralCmdFlags CmdFlags;

        /// <summary>
        /// 预留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 12)]
        public byte[] Res0;

        /// <summary>
        /// 拍电芯码完成
        /// </summary>
        public MstMsg_TakePhotoComplete TakePhotoComplete;

        /// <summary>
        /// 预留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        public byte[] Res3;

        /// <summary>
        /// 进工站
        /// </summary>
        public MstMsg_EnterStation EnterStation;

        /// <summary>
        /// 预留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        public byte[] Res1;

        /// <summary>
        /// 开始工作
        /// </summary>
        public MstMsg_StartInBox StartInBox;

        /// <summary>
        /// 预留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        public byte[] Res4;

        /// <summary>
        /// 单个入箱完成
        /// </summary>
        public MstMsg_SingleInBoxComplete SingleInBoxComplete;

        /// <summary>
        /// 预留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        public byte[] Res2;

        /// <summary>
        /// 全部入箱完成
        /// </summary>
        public MstMsg_InBoxComplete InBoxComplete;

    }
}
