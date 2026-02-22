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
    [Table("Proc_CheckPowerRecord")]
    public class Proc_CheckPowerRecord
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool IsDeleted { get; set; } = false;
        [MaxLength(200)]
        public string? Remark { get; set; }
      
        /// <summary>
        /// 刷卡账户
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 模块
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 工位
        /// </summary>
        [MaxLength(100)]
        public string? StationCode { get; set; }

        /// <summary>
        /// Pack码
        /// </summary>
        [MaxLength(100)]
        public string? PackCode { get; set; }

        /// <summary>
        /// 报警信息
        /// </summary>
        [MaxLength(200)]
        public string? Alarm { get; set; }

        public DateTime? CreateTime { get; set; }

    }
}
