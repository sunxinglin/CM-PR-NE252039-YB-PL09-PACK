using SqlSugar;
using System;
using System.Collections.Generic;

namespace RogerTech.Common.Models
{
    /// <summary>
    /// Unit项模板实体
    /// </summary>
    [SugarTable("Template_Module")]
    public class Template_Module
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
        public string ModuleUid { get; set; } //= Guid.NewGuid().ToString();

        /// <summary>
        /// Block料号
        /// </summary>      
        [SugarColumn(Length = 50, IsNullable = true)]
        public string BlockPn { get; set; }

        /// <summary>
        /// AGV托盘号
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int BlockSequence { get; set; }

        /// <summary>
        /// 关联的BlockId
        /// </summary>
            [SugarColumn(IsNullable = true)]
        public int BlockId { get; set; }

        /// <summary>
        /// Unit电芯数量
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int CellAmount { get; set; }

        /// <summary>
        /// Unit位置
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int ModuleIndex { get; set; }

        /// <summary>
        /// Unit类型，1Beam、2Pillar、9无
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public ModuleType ModuleType { get; set; }

        /// <summary>
        /// Unit极性
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public bool ModulePolarity { get; set; }

        /// <summary>
        /// Unit旋转
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public bool ModuleRotate { get; set; }

        /// <summary>
        /// Unit大面胶类型
        /// </summary>

        public ModuleFaceTapeType Side1Tape { get; set; }

        /// <summary>
        /// Unit侧面胶类型
        /// </summary>

        public ModuleSideTapeType Side2Tape { get; set; }

        /// <summary>
        /// Unit水冷板类型
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public WaterCoolType WaterCoolType { get; set; }

        /// <summary>
        /// Unit水冷管类型
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public WaterTubeType WaterTubeType { get; set; }

        /// <summary>
        /// Unit绝缘罩类型
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public SulationCoverType SulationCover { get; set; }

        /// <summary>
        /// 电芯集合
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(Template_Cell.ModuleId))]
        public List<Template_Cell> CellItems { get; set; } 
    }
} 