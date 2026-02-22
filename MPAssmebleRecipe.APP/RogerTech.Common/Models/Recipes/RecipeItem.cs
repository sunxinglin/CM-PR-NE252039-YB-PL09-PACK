
using RogerTech.Common.Models;
using SqlSugar;
using System;

namespace MPAssmebleRecipe.Models.Entities.Issues
{
    /// <summary>
    /// 以电芯为单位的生产配方项
    /// </summary>
    [SugarTable("Production_Recipe")]
    public class RecipeItem
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }

        /// <summary>
        /// 生产配方唯一标识
        /// </summary>
        [SugarColumn(Length = 50)]
        public Guid Uid { get; set; }

        /// <summary>
        /// Pack名称
        /// </summary>
        [SugarColumn(Length = 100)]
        public string PackName { get; set; }
        
        /// <summary>
        /// Pack料号
        /// </summary>
        [SugarColumn(Length = 50)]
        public string PackPn { get; set; }
        
        /// <summary>
        /// Block名称
        /// </summary>
        [SugarColumn(Length = 100)]
        public string BlockName { get; set; }
        
        /// <summary>
        /// Block料号
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string BlockPn { get; set; }
        
        /// <summary>
        /// Block条码
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string BlcokSn { get; set; }
        
        /// <summary>
        /// Block序号
        /// </summary>
        public int BlockIndex { get; set; }
        
        /// <summary>
        /// 是否插队
        /// </summary>
        public bool AdditionBlock { get; set; }
        
        /// <summary>
        /// Block开始标志
        /// </summary>
        //public bool BlockStartFlag { get; set; }
        
        /// <summary>
        /// Block结束标志
        /// </summary>
        //public bool BlockEndFlag { get; set; }
        
        /// <summary>
        /// 料道号
        /// </summary>
        //public int LineNum { get; set; }
        
        /// <summary>
        /// 模组数量
        /// </summary>
        public int ModuleCount { get; set; }
        
        /// <summary>
        /// 模组名称
        /// </summary>
        [SugarColumn(Length = 100, IsNullable = true)]
        public string ModuleName { get; set; }
        
        /// <summary>
        /// 模组GUID
        /// </summary>
        [SugarColumn(Length = 50)]
        public string ModuleGuid { get; set; }
        
        /// <summary>
        /// 模组料号
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string ModulePn { get; set; }//直接使用BlockPn
        
        /// <summary>
        /// 模组序号
        /// </summary>
        public int ModuleIndex { get; set; }
        
        /// <summary>
        /// 模组开始标识
        /// </summary>
        public bool ModuleStartFlag { get; set; }
        
        /// <summary>
        /// 模组结束标识
        /// </summary>
        public bool ModuleEndFlag { get; set; }
        
        /// <summary>
        /// 循环开始标识
        /// </summary>
        //public bool CycleStart { get; set; }
        
        /// <summary>
        /// 循环结束标识
        /// </summary>
        //public bool CycleEnd { get; set; }
        
        /// <summary>
        /// 是否需要缓存
        /// </summary>
        //public bool NeedCache { get; set; }
        
        /// <summary>
        /// 停止位置
        /// </summary>
        //public StopPostion StopPositon { get; set; }
        
        /// <summary>
        /// 抓取机器人
        /// </summary>
        //public GrabRobot GrabRobot { get; set; }
        
        /// <summary>
        /// 模组大面1胶类型
        /// </summary>
        //public ModuleSideType ModuleSide1Tapy { get; set; }
        
        /// <summary>
        /// 模组大面2胶类型
        /// </summary>
        //public ModuleSideType ModuleSide2Tapy { get; set; }
        
        /// <summary>
        /// 右限位片PN
        /// </summary>
        //[SugarColumn(Length = 50, IsNullable = true)]
        //public string RightStopper { get; set; }
        
        /// <summary>
        /// 左限位片PN
        /// </summary>
        //[SugarColumn(Length = 50, IsNullable = true)]
        //public string LeftStopper { get; set; }
        
        /// <summary>
        /// 前端板PN
        /// </summary>
        //[SugarColumn(Length = 50, IsNullable = true)]
        //public string FrontPanel { get; set; }
        
        /// <summary>
        /// 后端板PN
        /// </summary>
        //[SugarColumn(Length = 50, IsNullable = true)]
        //public string BackPanel { get; set; }
        
        /// <summary>
        /// 前绝缘罩PN
        /// </summary>
        //[SugarColumn(Length = 50, IsNullable = true)]
        //public string FrontsulationCover { get; set; }
        
        /// <summary>
        /// 后绝缘罩PN
        /// </summary>
        //[SugarColumn(Length = 50, IsNullable = true)]
        //public string BacksulationCover { get; set; }
        
        /// <summary>
        /// 下载索引
        /// </summary>
        public int DownLoadIndex { get; set; }
        
        /// <summary>
        /// 托盘号顺序
        /// </summary>
        public int PalletIndex { get; set; }
        
        /// <summary>
        /// 模组电芯数量
        /// </summary>
        public int CellCount { get; set; }
        
        /// <summary>
        /// 电芯条码
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string CellSn { get; set; } = " ";
        
        /// <summary>
        /// 电芯序号
        /// </summary>
        public int CellIndex { get; set; }
        
        /// <summary>
        /// 电芯极性
        /// </summary>
     //   public bool CellPolarity { get; set; }
        
        /// <summary>
        /// 大面1胶类型
        /// </summary>
        public int Side1Tape { get; set; }
        
        /// <summary>
        /// 大面2胶类型
        /// </summary>
        public int Side2Tape { get; set; }
        
        /// <summary>
        /// 电芯完成标识
        /// </summary>
        public bool CellComplete { get; set; }
        
        /// <summary>
        /// 模组下发完成标志
        /// </summary>
        public bool ModuleComplete { get; set; }
        
        /// <summary>
        /// 模组长度
        /// </summary>
        //public int ModuleLength { get; set; }
        
        /// <summary>
        /// 模组宽度
        /// </summary>
        //public int ModuleWidth { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime UpdateTime { get; set; } = DateTime.Now;
    }
} 