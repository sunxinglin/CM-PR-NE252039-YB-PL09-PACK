using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yee.Entitys.DTOS
{
    public class ChartModel
    {
        /// <summary>
        /// 日期
        /// </summary>
        public string? ProductName { set; get; }

        public int? ProductId { get; set; }
        public string? Date { get; set; }
        public int? Count { get; set; }

        /// <summary>
        /// 百分比
        /// </summary>
        public double? Percentage { set; get; }
    }
}
