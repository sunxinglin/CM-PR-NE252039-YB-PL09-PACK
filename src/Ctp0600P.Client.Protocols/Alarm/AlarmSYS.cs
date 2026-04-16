using MediatR;

using Yee.Entitys.DTOS;

namespace Ctp0600P.Client.Protocols;

public class AlarmSYS : INotification
{
    public AlarmSYS()
    {

    }
    public AlarmAction action { get; set; }
    public AlarmDTO alarmDTO { get; set; }
}