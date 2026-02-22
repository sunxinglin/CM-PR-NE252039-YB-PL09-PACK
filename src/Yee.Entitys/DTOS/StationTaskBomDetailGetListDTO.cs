using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yee.Entitys.DTOS
{
    public class StationTaskBomDetailGetListDTO
    {
        /// <summary>
        ///  pack码
        /// </summary>
        public string? PackPN { get; set; }
        /// <summary>
        /// 外部条码
        /// </summary>
        public string? OuterCode { get; set; }
        /// <summary>
        /// 每页条数
        /// </summary>
        public int Limit { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int Page { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
