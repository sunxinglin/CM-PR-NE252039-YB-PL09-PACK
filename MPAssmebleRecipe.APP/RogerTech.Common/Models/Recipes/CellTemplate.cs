using SqlSugar;
using System;

namespace RogerTech.Common.Models
{
    /// <summary>
    /// 电芯项模板实体
    /// </summary>
    [SugarTable("Template_Cell")]
    public class CellTemplate
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        
        /// <summary>
        /// 电芯唯一标识
        /// </summary>
        [SugarColumn(Length = 50)]
        public string CellUid { get; set; } = Guid.NewGuid().ToString();
        
        /// <summary>
        /// 电芯条码
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string CellSn { get; set; }
        
        /// <summary>
        /// 电芯料号
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string CellPn { get; set; }
        
        /// <summary>
        /// 父级料号
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string ParentPn { get; set; }
        
        /// <summary>
        /// 电芯序号
        /// </summary>
        public int CellIndex { get; set; }
        
        /// <summary>
        /// 极性
        /// </summary>
        public bool Polarity { get; set; }
        
        /// <summary>
        /// 大面1胶类型
        /// </summary>
        public CellSideTapeType Side1Tape { get; set; }
        
        /// <summary>
        /// 大面2胶类型
        /// </summary>
        public CellSideTapeType Side2Tape { get; set; }
        
        /// <summary>
        /// 是否混合料号
        /// </summary>
        public bool MixPn { get; set; }
        
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool Complete { get; set; }

        /// <summary>
        /// 关联的ModuleItemId
        /// </summary>
        public int ModuleItemId { get; set; }
    }
} 