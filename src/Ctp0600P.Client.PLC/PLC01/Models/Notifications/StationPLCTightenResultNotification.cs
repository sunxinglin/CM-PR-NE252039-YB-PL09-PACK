using MediatR;

namespace Ctp0600P.Client.PLC.PLC01.Models.Notifications;

public class StationPLCTightenResultNotification : INotification
{
    public int DeviceNo { get; set; }
    public int DeviceBrand { get; set; }
    public bool TightenResult { get; set; }
    public ushort PSet { get; set; }
    public float FinalTorque { get; set; }
    public ushort Constant1 { get; set; }
    public ushort TorqueTrend { get; set; }
    public float FinalAngle { get; set; }
    public ushort Constant2 { get; set; }
    public ushort AngleTrend { get; set; }
    public float TorqueRateMin { get; set; }
    public float TargetTorqueRate { get; set; }
    public float TorqueRateMax { get; set; }
    public float AngleMin { get; set; }
    public float TargetAngle { get; set; }
    public float AngleMax { get; set; }
}