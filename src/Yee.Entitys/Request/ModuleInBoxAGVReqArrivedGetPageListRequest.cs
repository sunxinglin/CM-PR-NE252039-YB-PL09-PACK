using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yee.Entitys.Request
{
    public class ModuleInBoxAGVReqArrivedGetPageListRequest : PageBaseRequest
    {
        public string? OuterGoodsCode { get; set; }

        public int? AGVNo { get; set; }

    }
}
