using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogerTech.Common.Models
{
    /// <summary>
    /// Module数据传输对象
    /// </summary>
    public class ModuleAddDto
    {
        /// <summary>
        /// 模组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 模组料号
        /// </summary>
        public string Pn { get; set; }

        /// <summary>
        /// Block ID
        /// </summary>
        public int BlockId { get; set; }

        /// <summary>
        /// BlockGroup ID
        /// </summary>
        public int BlockGroupId { get; set; }

        /// <summary>
        /// 电芯数量
        /// </summary>
        public int CellCount { get; set; }
    }
}
