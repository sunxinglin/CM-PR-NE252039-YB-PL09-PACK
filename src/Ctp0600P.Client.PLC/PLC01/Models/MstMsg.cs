using Ctp0600P.Client.PLC.Common;
using System.Runtime.InteropServices;

namespace Ctp0600P.Client.PLC.PLC01.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class MstMsg
    {
        public MstMsg() { }

        public MstMsg(MstMsg msg)
        {
            CmdFlags = msg.CmdFlags;
            Status = msg.Status;
            TightenStart = msg.TightenStart;
            TightenComplete = msg.TightenComplete;
        }

        public MstMsg_GeneralCmdFlags CmdFlags;

        public MstMsg_GeneralStatus Status;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 10)]
        public byte[] Res0;

        public MstMsg_TightenStart TightenStart;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 98)]
        public byte[] Res1;

        public MstMsg_TightenComplete TightenComplete;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        public byte[] Res2;

        public MstMsg_AGVArrive AGVArrive;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        public byte[] Res3;

        public MstMsg_AGVLeave AGVLeave;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        public byte[] Res4;

        public MstMsg_AGVBindPack AGVBindPack;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        public byte[] Res5;

        public MstMsg_ReleaseAGV ReleaseAGV;

        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 100)]
        //public byte[] Res6;


    }
}
