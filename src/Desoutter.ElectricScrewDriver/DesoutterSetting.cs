using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desoutter.ElectricScrewDriver
{
    public  class DesoutterSetting
    {
        public bool Enable { get; set; }

        public string IpAddr { get; set; } = "127.0.0.1";

        public int Port { get; set; }
    }

    public class DesoutterItemOption
    {
        public string Name { get; set; } = "Default";

        public bool Enable { get; set; }

        public string IpAddr { get; set; } = "127.0.0.1";

        public int Port { get; set; }
    }

}
