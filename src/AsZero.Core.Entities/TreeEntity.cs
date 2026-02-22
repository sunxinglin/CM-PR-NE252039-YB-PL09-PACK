using FutureTech.Dal.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsZero.Core.Entities
{
    /// <summary>
    /// 树状结构实体
    /// </summary>
    public  class TreeEntity  : FutureBaseEntity<int>
    {
        /// <summary>
        /// 节点语义ID
        /// </summary>
        [Description("节点语义ID")]
        public string CascadeId { get; set; }
        /// <summary>
        /// 功能模块名称
        /// </summary>
        [Description("名称")]
        public string Name { get; set; }

        /// <summary>
        /// 父节点流水号
        /// </summary>
        [Description("父节点流水号")]
        public int? ParentId { get; set; }
        /// <summary>
        /// 父节点名称
        /// </summary>
        [Description("父节点名称")]
        public string ParentName { get; set; }

    }
}
