using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctp0600P.Client.Protocols.AnyLoad
{
    public class AnyLoadConfigList
    {
        public List<AnyLoadConfig> ConfigList { get; set; }
    }

    public class AnyLoadConfig
    {
        public string AnyLoadIP { get; set; }
        public int AnyLoadPort { get; set; }
    }
}
