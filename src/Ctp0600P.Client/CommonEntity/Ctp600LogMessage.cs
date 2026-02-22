using AsZero.Core.Services.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctp0600P.Client.CommonEntity
{
    public class Ctp600LogMessage : LogMessage
    {
        public string ModelName
        {
            get;
            set;
        }
    }
}