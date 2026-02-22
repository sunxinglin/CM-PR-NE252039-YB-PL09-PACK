
using SqlSugar;
using System;

namespace RogerTech.Common.Models
{
    /// <summary>
    /// 以电芯为单位的生产配方项
    /// </summary>
    [SugarTable("Production_Recipe")]
    public class Production_Recipe
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }

        /// <summary>
        /// 是否下发
        /// </summary>
        public bool Issued { get; set; }
        /// <summary>
        /// 是否绑定
        /// </summary>
        public bool IsBinding { get; set; }

        /// <summary>
        /// 配方关联ID
        /// </summary>
        public int RecipeId { get; set; }
        /// <summary>
        /// 配方步
        /// </summary>
        public int RecipeIndex { get; set; }

        /// <summary>
        /// Block料号
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = false)]
        public string BlockPn { get; set; }
        /// <summary>
        /// Block条码
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string BlcokSn { get; set; }

        /// <summary>
        /// 电芯条码
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string CellSn { get; set; }

        /// <summary>
        /// 托盘号
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? PalletIndex { get; set; }

        /// <summary>
        /// 电芯极性是否旋转,1、-1
        /// </summary>
        [SugarColumn(IsNullable = true)] 
        public int CellPolarity { get; set; }

        /// <summary>
        /// 大面胶类型
        /// </summary>
        [SugarColumn(IsNullable = true)] 
        public int Side1Tape { get; set; }

        /// <summary>
        /// 小面胶类型
        /// </summary>
        [SugarColumn(IsNullable = true)] 
        public int Side2Tape { get; set; }

        /// <summary>
        /// AGV转运Blcok数量
        /// </summary>
        [SugarColumn(IsNullable = true)] 
        public int BlockCount { get; set; }

        /// <summary>
        /// Blcok在Pack中顺序
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int BlockIndex { get; set; }

        /// <summary>
        /// Block下Pillar数量
        /// </summary>
        [SugarColumn(IsNullable = true)] 
        public int PillarCount { get; set; }

        /// <summary>
        /// Block下Pillar顺序
        /// </summary>
        [SugarColumn(IsNullable = true)] 
        public int PillarIndex { get; set; }

        /// <summary>
        /// Pillar是否旋转
        /// </summary>
        public bool PillarRotate { get; set; }

        /// <summary>
        /// 单列堆叠电芯数量
        /// </summary>
        [SugarColumn(IsNullable = true)] 
        public int PillarCellCount { get; set; }

        /// <summary>
        /// 配方循环开始
        /// </summary>
        public bool RecipeStrat { get; set; }

        /// <summary>
        /// 配方循环结束
        /// </summary>
        public bool RecipeEnd { get; set; }

        /// <summary>
        /// Pillar配方开始
        /// </summary>
        public bool PillarStart { get; set; }

        /// <summary>
        /// Pillar配方结束
        /// </summary>
        public bool PillarEnd { get; set; }

        /// <summary>
        /// 拆分类型
        /// </summary>
        public bool SplitType { get; set; }

        /// <summary>
        /// 配方剩余数量
        /// </summary>
        public int RecipeRemaining { get; set; }

        /// <summary>
        /// 层号
        /// </summary>
        public int LayerNum { get; set; }

        /// <summary>
        /// 下一蓝本
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string NextRecipe { get; set; }

        /// <summary>
        /// 生产结束
        /// </summary>
        public bool ProductionEnd { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(IsNullable = false, DefaultValue = "CURRENT_TIMESTAMP")]
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
} 