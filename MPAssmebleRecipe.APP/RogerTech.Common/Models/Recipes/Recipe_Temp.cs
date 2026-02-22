
using SqlSugar;
using System;

namespace RogerTech.Common.Models
{
    /// <summary>
    /// 正常生产配方
    /// </summary>
    [SugarTable("Recipe_Temp")]
    public class Recipe_Temp
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }

        /// <summary>
        /// Block唯一标识
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = false)]
        public string Blockuid { get; set; }

        /// <summary>
        /// BlockPN
        /// </summary>
        [SugarColumn(Length = 20, IsNullable = false)]
        public string BlockPN { get; set; }

        /// <summary>
        /// Block数量
        /// </summary>
        public int BlockAmount { get; set; }

        /// <summary>
        /// 配方顺序
        /// </summary>
        public int RecipeIndex { get; set; }

        /// <summary>
        /// 配方类型
        /// </summary>
        public RecipeTypeEnum RecipeType { get; set; }

        /// <summary>
        /// 层号
        /// </summary>
        public int LayerNum { get; set; }

        /// <summary>
        /// 是否生效，清线、下发置为0
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 归属拉线
        /// </summary>
        [SugarColumn(Length = 20, IsNullable = false)]
        public string AttributionLine { get; set; }

        /// <summary>
        /// 预分配拉线
        /// </summary>
        [SugarColumn(Length = 20, IsNullable = true)]
        public string PreallocationLine { get; set; }

        /// <summary>
        /// 下发拉线
        /// </summary>
        [SugarColumn(Length = 20, IsNullable = true)]
        public string IssueLine { get; set; }

        /// <summary>
        /// 预分配时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? PreallocationTime { get; set; }

        /// <summary>
        /// 下发时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? IssueTime { get; set; }
    }
} 