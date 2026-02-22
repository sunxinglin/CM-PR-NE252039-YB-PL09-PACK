using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic.Protocols.LowerBoxGlue.Models.Wraps
{
    public class DealReqReGlueCompleteWraps : INotification
    {
        public int VectorCode { get; set; } = 1;
        public string PackCode { get; set; } = "";
        public string StartTime { get; set; } = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
    }
}
