using SqlSugar;
using System;
using System.Collections.Generic;

namespace RogerTech.Common.Models
{
    /// <summary>
    /// Block项模板实体
    /// </summary>
    [SugarTable("Template_Block")]
    public class Template_Block
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        
        /// <summary>
        /// Block唯一标识
        /// </summary>
        [SugarColumn(Length = 50)]
        public string BlockUid { get; set; } //= Guid.NewGuid().ToString();

        /// <summary>
        /// Block名称       
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string BlockName { get; set; }
        
        /// <summary>
        /// Block料号
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string BlockPn { get; set; }

        /// <summary>
        /// 关联的PackId
        /// </summary>
        /// 
        [SugarColumn(IsNullable = true)]
        public int PackId { get; set; }

        /// <summary>
        /// 父级料号
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string ParentPn { get; set; }

        /// <summary>
        /// 归属拉线[ABCD]
        /// </summary>
        [SugarColumn(Length = 20, IsNullable = true)]
        public string AttributionLine { get; set; }

        /// <summary>
        /// AGV编号
        /// </summary>
        /// 
        [SugarColumn(IsNullable = true)]
        public int BlockSequence { get; set; }

        /// <summary>
        /// AGV上Block序号
        /// </summary>
        /// 
        [SugarColumn(IsNullable = true)]
        public int BlockIndex { get; set; }

        /// <summary>
        /// AGV上Block数量
        /// </summary>
        /// 
        [SugarColumn(IsNullable = true)]
        public int BlockAmount { get; set; }

        /// <summary>
        /// 模组数量
        /// </summary>
        /// 
        [SugarColumn(IsNullable = true)]
        public int ModuleAmount { get; set; }

        /// <summary>
        /// Pack层号
        /// </summary>
        /// 
        [SugarColumn(IsNullable = true)]
        public int LayerNum { get; set; }

        /// <summary>
        /// 入箱料道号
        /// </summary>
        /// 
        [SugarColumn(IsNullable = true)]
        public int LineNum { get; set; }

        /// <summary>
        /// 限位片，true贴、false不贴
        /// </summary>
        /// 
        [SugarColumn(IsNullable = true)]
        public bool LimitPlate { get; set; } = false;

        /// <summary>
        /// 拆分类型，true拆分、false不拆分
        /// </summary>
        /// 
        [SugarColumn(IsNullable = true)]
        public bool SplitType { get; set; } = false;

        /// <summary>
        /// 模组集合
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(Template_Module.BlockId))]
        public List<Template_Module> ModuleItems { get; set; } 
    }
} 