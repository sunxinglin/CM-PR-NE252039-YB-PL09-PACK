using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsZero.Core.Services.Repos
{
    public class CommonRequest
    { 
    }
    public class DeleteByIdsInput
    {
        public int[] Ids { get; set; } = Array.Empty<int>();
    }

    public class PageReq
    {
        public int page { get; set; }
        public int limit { get; set; }

        public string? key { get; set; }

        public PageReq()
        {
            page = 1;
            limit = 10;
        }
    }
}
