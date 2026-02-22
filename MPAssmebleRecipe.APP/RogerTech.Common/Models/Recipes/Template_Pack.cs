using SqlSugar;
using System;
using System.Collections.Generic;

namespace RogerTech.Common.Models
{
    /// <summary>
    /// Pack模板实体
    /// </summary>
    [SugarTable("Template_Pack")]
    public class Template_Pack
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        
        /// <summary>
        /// Pack唯一标识
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string PackUid { get; set; } = Guid.NewGuid().ToString();
        
        /// <summary>
        /// Pack名称
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string PackName { get; set; }
        
        /// <summary>
        /// Pack料号
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string PackPn { get; set; }

        /// <summary>
        /// 总层数
        /// </summary>
        /// 
        [SugarColumn(IsNullable = true)]
        public int LayerTotal { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public DateTime CreateTime { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 最后修改时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? ModifyTime { get; set; }

        /// <summary>
        /// Block集合
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(Template_Block.PackId))]

        public List<Template_Block> BlockItems { get; set; } 
    }
} 