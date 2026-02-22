using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogerTech.Common.Models
{
    public class LogModelDto
    {
        public string InterfaceName { get; set; } //接口名称分类
        public int Page { get; set; } = 1;        // 第几页
        public int PageSize { get; set; } = 10;   // 每页展示数量
        public DateTime StartTime { get; set; }   //开始时间
        public DateTime EndTime { get; set; }     //结束时间
    }
}
