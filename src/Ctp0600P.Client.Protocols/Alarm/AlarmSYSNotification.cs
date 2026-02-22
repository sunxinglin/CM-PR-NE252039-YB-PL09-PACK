using MediatR;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.DTOS;

namespace Ctp0600P.Client.Protocols
{
    public class AlarmSYSNotification : AlarmDTO, INotification
    {

        public AlarmAction action { get; set; }

        public AlarmExtra.TightenNG? TightenNGExtra { get; set; }

    }
}
