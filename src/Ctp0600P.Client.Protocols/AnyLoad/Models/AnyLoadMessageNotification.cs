using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctp0600P.Client.Protocols.AnyLoad
{
    public class AnyLoadMessageNotification : INotification
    {
        public AnyLoadMessageNotification()
        {

        }

        public string Message { get; set; }
    }
}
