using FutureTech.Protocols;

using MediatR;

using System.Runtime.InteropServices;

namespace Automatic.Protocols.UpperCoverTighten.Models.Datas
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class TighteningData
    {
        public TighteningFlag TighteningResultFlag;

        [Endian(Endianness.BigEndian)]
        public ushort Pset_selected;

        [Endian(Endianness.BigEndian)]
        public float Final_torque;

        [Endian(Endianness.BigEndian)]
        public ushort Constant1;

        [Endian(Endianness.BigEndian)]
        public ushort Torque_trend;

        [Endian(Endianness.BigEndian)]
        public float Final_angle;

        [Endian(Endianness.BigEndian)]
        public ushort Constant2;

        [Endian(Endianness.BigEndian)]
        public ushort Angle_trend;

        [Endian(Endianness.BigEndian)]
        public float TorqueRate_Min;

        [Endian(Endianness.BigEndian)]
        public float TargetTorqueRate;

        [Endian(Endianness.BigEndian)]
        public float TorqueRate_Max;

        [Endian(Endianness.BigEndian)]
        public float Angle_Min;

        [Endian(Endianness.BigEndian)]
        public float TargetAngle;

        [Endian(Endianness.BigEndian)]
        public float Angle_Max;

        public bool TightenPass => TighteningResultFlag.HasFlag(TighteningFlag.Tightening_OK) && !TighteningResultFlag.HasFlag(TighteningFlag.Tightening_NOK);
        public ushort Pset => Pset_selected;
        public float TorqueValue => Final_torque;
        public float AngleValue => Final_angle;
        public float TorqueMin => TorqueRate_Min;
        public float TorqueMax => TorqueRate_Max;
        public float AngleMin => Angle_Min;
        public float AngleMax => Angle_Max;
    }
}
