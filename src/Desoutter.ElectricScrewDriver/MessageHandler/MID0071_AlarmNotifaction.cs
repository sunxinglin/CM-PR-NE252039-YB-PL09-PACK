using Desoutter.ElectricScrewDriver.MidMessage;
using MediatR;

namespace Desoutter.ElectricScrewDriver.MessageHandler
{
    public  class MID0071_AlarmNotifaction : INotification
    {
        public MID0071_AlarmNotifaction()
        {

        }

        public string DeviceNo { get; set; }
        public MID0071_Alarm Alarm { get; set; }

    }
}
