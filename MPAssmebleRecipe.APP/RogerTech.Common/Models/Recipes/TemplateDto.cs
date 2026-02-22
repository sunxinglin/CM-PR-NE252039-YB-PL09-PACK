
using SqlSugar;
using System;

namespace RogerTech.Common.Models
{
    /// <summary>
    /// 以电芯为单位的生产配方项
    /// </summary>
    public class TemplateDto
    {
        /// <summary>
        /// Block料号
        /// </summary>
        public string BlockPn { get; set; }
        /// <summary>
        /// AGV转运Blcok数量
        /// </summary>
        public int BlockCount { get; set; }
        /// <summary>
        /// Blcok在Pack中顺序
        /// </summary>
        public int BlockIndex { get; set; }
        /// <summary>
        /// Block下Pillar数量
        /// </summary>
        public int ModuleColums { get; set; }
        /// <summary>
        /// 层号
        /// </summary>
        public int LayerNum { get; set; }
        /// <summary>
        /// 入箱料道编号
        /// </summary>
        public int LineNum { get; set; }
        /// <summary>
        /// 拆分类型
        /// </summary>
        public bool SplitType { get; set; }
        /// <summary>
        /// 限位片
        /// </summary>
        public bool LimitPlate { get; set; }


        /// <summary>
        /// 电芯数量
        /// </summary>
        public int CellCount { get; set; }
        /// <summary>
        /// 模组序号
        /// </summary>
        public int ModuleIndex { get; set; }
        /// <summary>
        /// 模组大面胶类型
        /// </summary>
        public ModuleFaceTapeType ModuleFaceTape { get; set; }
        /// <summary>
        /// 模组侧面胶类型
        /// </summary>
        public ModuleSideTapeType ModuleSideTape { get; set; }
        /// <summary>
        /// 模组类型，1Beam、2Pillar、9无
        /// </summary>
        public ModuleType ModuleType { get; set; }
        /// <summary>
        /// 模组极性，true正极、false负极
        /// </summary>
        public bool ModulePolarity { get; set; } = false;
        /// <summary>
        /// 模组旋转
        /// </summary>
        public bool ModuleRotate { get; set; }
        /// <summary>
        /// 水冷板类型，默认9
        /// </summary>
        public WaterCoolType WaterCoolType { get; set; }
        /// <summary>
        /// 水冷管类型，默认9
        /// </summary>
        public WaterTubeType WaterTubeType { get; set; }
        /// <summary>
        /// 贴绝缘罩，默认9
        /// </summary>
        public SulationCoverType SulationCover { get; set; }


        /// <summary>
        /// 电芯序号
        /// </summary>
        public int CellIndex { get; set; }
        /// <summary>
        /// 极性
        /// </summary>
        public bool CellPolarity { get; set; }
        /// <summary>
        /// 大面胶类型
        /// </summary>
        public CellFaceTapeType CellSideTape1 { get; set; }
        /// <summary>
        /// 侧面胶类型
        /// </summary>
        public CellSideTapeType CellSideTape2 { get; set; }
    }
} 