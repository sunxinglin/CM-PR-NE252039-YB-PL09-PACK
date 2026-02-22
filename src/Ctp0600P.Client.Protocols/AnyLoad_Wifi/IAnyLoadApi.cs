using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctp0600P.Client.Protocols.AnyLoad_Wifi
{
    public interface IAnyLoadApi
    {
        Task ReadCurrentWeightData();
    }
}