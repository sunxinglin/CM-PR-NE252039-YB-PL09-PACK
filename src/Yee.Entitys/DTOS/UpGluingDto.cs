using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yee.Entitys.DTOS
{
    public class UpGluingDto
    {
        public string packCode { get; set; }
        /// <summary>
        /// 需要修改的涂胶时间
        /// </summary>
        public DateTime glueTime { get; set; }
        /// <summary>
        /// 需要修改时间的下箱体码
        /// </summary>
 
    }
}
