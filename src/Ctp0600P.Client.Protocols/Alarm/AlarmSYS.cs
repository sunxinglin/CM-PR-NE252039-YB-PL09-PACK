using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Entitys.DTOS;

namespace Ctp0600P.Client.Protocols
{
    public class AlarmSYS : INotification
    {
        public AlarmSYS()
        {

        }
        public AlarmAction action { get; set; }
        public AlarmDTO alarmDTO { get; set; }
    }
}
