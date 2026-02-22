using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctp0600P.Client.Protocols.AnyLoad
{
    public interface IAnyLoadApi
    {
        Task ReadCurrentWeightData();
    }
}