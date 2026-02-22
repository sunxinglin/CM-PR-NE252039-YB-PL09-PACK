using SqlSugar;
using System;

namespace RogerTech.Common.Models
{
    /// <summary>
    /// 电芯项模板实体
    /// </summary>
    [SugarTable("Template_Cell")]
    public class Template_Cell
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
        public string CellUid { get; set; } //= Guid.NewGuid().ToString();

        /// <summary>
        /// Block料号
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string BlockPn { get; set; }

        /// <summary>
        /// 关联的ModuleId
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int ModuleId { get; set; }

        /// <summary>
        /// 电芯序号
        /// </summary>
        public int CellIndex { get; set; }
        
        /// <summary>
        /// 电芯极性
        /// </summary>
        public bool Polarity { get; set; }

        /// <summary>
        /// 电芯大面胶类型
        /// </summary>
        public CellFaceTapeType Side1Tape { get; set; }

        /// <summary>
        /// 电芯侧面胶类型
        /// </summary>
        public CellSideTapeType Side2Tape { get; set; }
    }
} 