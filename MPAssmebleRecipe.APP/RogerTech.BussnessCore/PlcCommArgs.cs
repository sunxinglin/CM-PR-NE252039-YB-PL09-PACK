using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogerTech.Tool;

namespace RogerTech.BussnessCore
{
    public class PlcCommArgs:EventArgs
    {
        public Group PlcGroup { get; set; }
        public object TagValue { get; set; }

        public PlcCommArgs(Group group, object tagValue)
        {
            this.PlcGroup = group;
            this.TagValue = tagValue;
        }
    }
}
