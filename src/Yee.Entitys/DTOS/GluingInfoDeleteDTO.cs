using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yee.Entitys.DTOS
{
    public class GluingInfoDeleteDTO
    {
        /// <summary>
        /// 主记录Id
        /// </summary>
        public int[]? MainIds { get; set; }
        /// <summary>
        /// 涂胶数据Id
        /// </summary>
        public int[]? GluingInfoIds { get; set; }

    }
}
