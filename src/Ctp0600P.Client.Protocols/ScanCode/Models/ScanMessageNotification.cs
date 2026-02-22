using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctp0600P.Client.Protocols
{
    public class ScanMessageNotification : INotification
    {
        public ScanMessageNotification()
        {

        }

        public string Message { get; set; }
    }
}
