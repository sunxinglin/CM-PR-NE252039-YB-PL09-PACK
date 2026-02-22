using Automatic.Protocols.Common;
using System.Runtime.InteropServices;

namespace Automatic.Protocols.ShoulderGlue.Models
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
        /// 进工位
        /// </summary>
        public MstMsg_EnterStation EnterStation;

        /// <summary>
        /// 预留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        public byte[] Res1;

        /// <summary>
        /// 开始涂胶
        /// </summary>
        public MstMsg_StartGlue StartGlue;

        /// <summary>
        /// 预留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        public byte[] Res2;

        /// <summary>
        /// 涂胶完成
        /// </summary>
        public MstMsg_GlueComplete GlueComplete;

        /// <summary>
        /// 预留
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        public byte[] Res3;

        /// <summary>
        /// 首件
        /// </summary>
        public MstMsg_FirstArticle FirstArticle;

    }
}
