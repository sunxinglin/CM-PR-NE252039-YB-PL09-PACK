using MediatR;

namespace Ctp0600P.Client.PLC.PLC01.Models.Notifications
{
    public class StationPLCLeakResultNotification : INotification
    {
        public string LeakStartTime { get; set; }

        public string LeakCompleteTime { get; set; }

        public float LeakTotalPower { get; set; }

        public float LeakProportionalPressure { get; set; }

        public float LeakAdkerPressure { get; set; }

        public ushort LeakTime { get; set; }

        public ushort LeakKeepTime { get; set; }

        public float LeakPressPower { get; set; }

        public float LeakKeepPressPower { get; set; }

        public ushort LeakRealTime { get; set; }
    }
}
