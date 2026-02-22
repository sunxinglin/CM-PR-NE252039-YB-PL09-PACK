using MediatR;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DTOS;

namespace Ctp0600P.Client.Protocols
{
    public  class Alarm_IONotification : INotification
    {
        public Alarm_IONotification()
        {

        }
        public AlarmAction action { get; set; }
        public AlarmDTO alarmDTO { get; set; }
    }
}
