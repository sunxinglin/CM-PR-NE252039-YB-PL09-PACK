using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yee.Entitys.Request
{
    public class PageBaseRequest
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
    }
}
