using System.Runtime.InteropServices;

using FutureTech.Protocols;

namespace Ctp0600P.Client.PLC.PLC01.Models.Datas;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class TightenData
{
    public TightenFlag TighteningResultFlag;

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

}