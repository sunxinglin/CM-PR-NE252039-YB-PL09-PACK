using System.Runtime.InteropServices;

using Ctp0600P.Client.PLC.Common;

namespace Ctp0600P.Client.PLC.PLC01.Models
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
        /// 开始拧紧
        /// </summary>
        public DevMsg_TightenStart TightenStart;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        public byte[] Res1;

        /// <summary>
        /// 拧紧完成
        /// </summary>
        public DevMsg_TightenComplete TightenComplete;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        public byte[] Res2;

        /// <summary>
        /// AGV到站
        /// </summary>
        public DevMsg_AGVArrive AGVArrive;


        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        public byte[] Res3;

        /// <summary>
        /// AGV离站
        /// </summary>
        public DevMsg_AGVLeave AGVLeave;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        public byte[] Res4;

        /// <summary>
        /// AGV绑定条码
        /// </summary>
        public DevMsg_AGVBindPack AGVBindPack;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        public byte[] Res5;

        /// <summary>
        /// 放行AGV
        /// </summary>
        public DevMsg_ReleaseAGV ReleaseAGV;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        public byte[] Res6;

        /// <summary>
        /// AGV状态
        /// </summary>
        public DevMsg_AGVStatus AGVStatus;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 10)]
        public byte[] Res7;

        /// <summary>
        /// 拧紧枪状态
        /// </summary>
        public DevMsg_TightenGunStatus TightenGunStatus;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        public byte[] Res8;

        public DevMsg_LeakStart LeakStart;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        public byte[] Res9;

        public DevMsg_LeakComplete LeakComplete;

    }
}
