using Automatic.Protocols.Common;
using Automatic.Protocols.TerminalReshape.Models.WorkRequireFlags;
using FutureTech.Protocols;

using System.Runtime.InteropServices;

namespace Automatic.Protocols.TerminalReshape.Models
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class MstMsg
    {
        public MstMsg_GeneralCmdFlags CmdFlags;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 10)]
        public byte[] Res0;

        public MstMsgAckVectorEnterFlag MstMsgAckVectorEnterFlag;

        [Endian(Endianness.BigEndian)]
        public ushort EnterErrorCode;
        [Endian(Endianness.BigEndian)]
        public ushort EnterReshapeLevel;

        [Endian(Endianness.BigEndian)]
        public uint LeftTime;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 26)]
        public byte[] Res1;

        public MstMsgAckStartReshapeFlag MstMsgAckStartReshapeFlag;
        [Endian(Endianness.BigEndian)]
        public ushort StartErrorCode;
        [Endian(Endianness.BigEndian)]
        public ushort StartReshapeLevel;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 24)]
        public byte[] Res2;

        public MstMsgAckComplateReshapeFlag MstMsgAckComplateReshapeFlag;
        [Endian(Endianness.BigEndian)]
        public ushort ComplateErrorCode;
    }
}
