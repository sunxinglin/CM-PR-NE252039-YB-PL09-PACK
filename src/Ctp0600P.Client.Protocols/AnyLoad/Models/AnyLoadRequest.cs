using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctp0600P.Client.Protocols
{
    public class AnyLoadRequest : INotification 
    {
        public string AnyLoadContext { get; set; } = "";
        public string AnyLoadPortName { get; set; } = "";
    }
}
