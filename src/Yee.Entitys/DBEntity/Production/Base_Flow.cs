using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Entitys.Common;
using Yee.Entitys.Production;

namespace Yee.Entitys.Production
{
    [Table("Base_Flow")]
    public class Base_Flow : CommonData
    {
        /// <summary>
        /// 编码
        /// </summary>
        [MaxLength(200)]
        public string? Code { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(200)]
        public string? Name { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        [MaxLength(200)]
        public string? Version { get; set; }

        /// <summary>
        /// 关联产品
        /// </summary>
        public Base_Product? Product { get; set; }
        /// <summary>
        /// 产品主键
        /// </summary>
        public int ProductId { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
