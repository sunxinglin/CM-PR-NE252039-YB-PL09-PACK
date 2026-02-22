using Desoutter.ElectricScrewDriver.MidMessage;
using MediatR;

namespace Desoutter.ElectricScrewDriver.MessageHandler
{
    public  class MID0061_LastTighteningResultDataNotifaction : INotification
    {
        public MID0061_LastTighteningResultDataNotifaction()
        {

        }

        public string DeviceNo { get; set; }

        public MID0061_LastTighteningResultData LastTighteningResultData { get; set; }

    }

    public class MID0061_LastTighteningResultData_BSNotifaction : INotification
    {
        public MID0061_LastTighteningResultData_BSNotifaction()
        {

        }

        public string DeviceNo { get; set; }

        public MID0061_LastTighteningResultData_BS LastTighteningResultData { get; set; }

    }
}
