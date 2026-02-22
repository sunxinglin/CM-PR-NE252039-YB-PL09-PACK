using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic.Protocols.LowerBoxGlue.Models.Wraps
{
    public class DealReqReGlueStartWraps : INotification
    {
        public int VectorCode { get; set; } = 1;
        public string PackCode { get; set; } = "";
    }
}
