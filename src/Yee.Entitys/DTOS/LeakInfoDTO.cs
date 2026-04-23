using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yee.Entitys.DTOS
{
    public class LeakInfoDTO
    {

        public int Limit { get; set; }
        public int Page { get; set; }
        public string? PackPN { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }

    }

    /// <summary>
    /// 充气数据重传参数
    /// </summary>
    public class LeakReuploadDto
    {
        /// <summary>
        /// Pack码
        /// </summary>
        public string? PackCode { get; set; }

        /// <summary>
        /// 工位代码
        /// </summary>
        public string? StationCode { get; set; }
    }
}