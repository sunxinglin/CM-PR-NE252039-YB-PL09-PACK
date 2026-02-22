using Automatic.Protocols.Common;
using Automatic.Protocols.TerminalReshape.Models.WorkRequireFlags;
using FutureTech.Protocols;

using System.Runtime.InteropServices;

namespace Automatic.Protocols.TerminalReshape.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class DevMsg
    {
        public const int GlueDataSize = 16;
        public const int GlueDataCount = 30;

        public const int WeightDataSize = 4;
        public const int WeightDataCount = 30;

        /// <summary>
        /// 通用命令
        /// </summary>
        public DevMsg_GeneralCmdFlags CmdFlags;

        /// <summary>
        /// 通用状态
        /// </summary>
        public DevMsg_GeneralStatus Status;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 10)]
        public byte[] Res0;


        public DevMsgReqVectorEnterFlag DevMsgReqVectorEnterFlag;

        [Endian(Endianness.BigEndian)]
        public ushort EnterVectorCode;

        public String40 EnterPackCode;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 22)]
        public byte[] Res1;

        public DevMsgReqStartReshapeFlag DevMsgReqStartReshapeFlag;

        [Endian(Endianness.BigEndian)]
        public ushort StartVectorCode;

        public String40 StartPackCode;


        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 26)]
        public byte[] Res2;

        public DevMsgReqComplateReshapeFlag DevMsgReqComplateReshapeFlag;

        [Endian(Endianness.BigEndian)]
        public ushort ComplateVectorCode;

        public String40 ComplatePackCode;

        [Endian(Endianness.BigEndian)]
        public ushort ReshapeStartYear;
        [Endian(Endianness.BigEndian)]
        public ushort ReshapeStartMonth;
        [Endian(Endianness.BigEndian)]
        public ushort ReshapeStartDay;
        [Endian(Endianness.BigEndian)]
        public ushort ReshapeStartHour;
        [Endian(Endianness.BigEndian)]
        public ushort ReshapeStartMinute;
        [Endian(Endianness.BigEndian)]
        public ushort ReshapeStartSecond;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 136)]
        public byte[] ReshapeData;

        public string ReshapeStartTime => $"{ReshapeStartYear}/{ReshapeStartMonth}/{ReshapeStartDay} {ReshapeStartHour}:{ReshapeStartMinute}:{ReshapeStartSecond}";
    }
}
