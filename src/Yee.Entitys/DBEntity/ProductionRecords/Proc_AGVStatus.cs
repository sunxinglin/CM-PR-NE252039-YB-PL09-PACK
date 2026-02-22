using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.Common;
using Yee.Entitys.Production;

namespace Yee.Entitys.DBEntity
{
    /// <summary>
    /// AGV状态表
    /// </summary>
    [Table("Proc_AGVStatus")]
    public class Proc_AGVStatus : CommonData
    {
        /// <summary>
        /// 小车号
        /// </summary>
        public int AGVNo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Behavior { get; set; }

        /// <summary>
        /// 工位号
        /// </summary>
        [MaxLength(100)]
        public string? StationCode { get; set; }

        /// <summary>
        /// 绑定的Pack码
        /// </summary>
        [MaxLength(100)]
        public string? PackPN { get; set; }

        /// <summary>
        /// 绑定的产品类型
        /// </summary>
        [MaxLength(100)]
        public string? ProductType { get; set; }

        /// <summary>
        /// 台车码
        /// </summary>
        [MaxLength(100)]
        public string? HolderBarCode { get; set; }
    }
}
