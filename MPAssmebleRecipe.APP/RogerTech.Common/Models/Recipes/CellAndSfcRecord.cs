using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace RogerTech.Common.Models
{
    /// <summary>
    /// 应用配置实体
    /// </summary>
    [SugarTable("Cell_And_Sfc_Record")]
    public class CellAndSfcRecord
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }

        /// <summary>
        /// 配置分类
        /// </summary>
        [SugarColumn(Length = 50)]
        public string Sfc { get; set; }

        /// <summary>
        /// 配置键
        /// </summary>
        [SugarColumn(Length = 50)]
        public string CellSn { get; set; }
    }
}
