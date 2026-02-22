using System.ComponentModel;

namespace RogerTech.Common.Models
{
    /// <summary>
    /// 模组列数
    /// </summary>
    public enum ModuleColumns
    {
        One = 1,
        Two = 2,
        Three = 3
    }

    /// <summary>
    /// 模组类型
    /// </summary>
    public enum ModuleType
    {
        [Description("无")]
        None = 0,
        [Description("单列")]
        Pillar = 1,
        [Description("底部水冷")]
        Beam = 2,
    }

    /// <summary>
    /// 模组大面胶类型
    /// </summary>
    public enum ModuleFaceTapeType
    {
        不贴 = 0,
        黑色胶 = 1,
        白色胶 = 2,
        白色框胶 = 3,
    }
    /// <summary>
    /// 模组侧面胶类型
    /// </summary>
    public enum ModuleSideTapeType
    {
        不贴 = 0,
        小面胶 = 1,
    }

    /// <summary>
    /// 电芯大面胶类型
    /// </summary>
    public enum CellFaceTapeType
    {
        不贴 = 0,
        黑色胶 = 1,
        白色胶 = 2,
        白色框胶 = 3,
    }
    /// <summary>
    /// 电芯侧面胶类型
    /// </summary>
    public enum CellSideTapeType
    {
        不贴 = 0,
        小面胶 = 1,
    }

    /// <summary>
    /// 配方类型
    /// </summary>
    public enum RecipeTypeEnum
    {
        开拉 = 1,
        正常 = 2,
        收拉 = 3
    }

    /// <summary>
    /// 水冷板类型
    /// </summary>
    public enum WaterCoolType
    {
        无 = 9,
        普通 = 1,
        特殊直头 = 2,
        特殊平头 = 3,
    }
    /// <summary>
    /// 水冷管类型
    /// </summary>
    public enum WaterTubeType
    {
        无 = 9,
        普通 = 1,
        特殊1 = 2,
        特殊2 = 3,
    }
    /// <summary>
    /// 绝缘罩类型
    /// </summary>
    public enum SulationCoverType
    {
        不贴 = 9,
        贴绝缘罩返回 = 1,
        贴绝缘罩不返回 = 2,
    }

    /// <summary>
    /// 抓取位置
    /// </summary>
    public enum StopPostion
    {
        None = 0,
        A = 1,
        B = 2,
        C = 3
    }

    /// <summary>
    /// 转线抓取机器人
    /// </summary>
    public enum GrabRobot
    {
        None = 0,
        RobotA = 1,
        RobotB = 2,
    }

    /// <summary>
    /// 插队标识
    /// </summary>
    public enum Addition
    {
        Block = 2,  //当前blockGroup类型做完进行插队 --弃用，改为当前配方作完进行插队
        Pack = 1    //当前Pack做完进行插队（当前生成的配方做完）
    }

    /// <summary>
    /// MES参数类型
    /// </summary>
    public enum MesDataType
    {
        BOOLEAN = 0,
        NUMBER = 1,
        STRING = 2
    }

    /// <summary>
    /// MES参数属性
    /// </summary>
    public enum MesDataProperty
    {
        属性1 = 1,
        属性2 = 2
    }

    /// <summary>
    /// MES参数启用
    /// </summary>
    public enum MesParamEnableType
    {
        停用 = 0,
        启用 = 1
    }

    /// <summary>
    /// 物料装配类型
    /// </summary>
    public enum MaterialAssembleType
    {
        批次精追装配 = 1,
        非批次精追装配 = 2,
        批次装配 = 3
    }

    /// <summary>
    /// 是否启用，1启用，0停用
    /// </summary>
    public enum IsEnableType
    {
        启用 = 1,
        停用 = 0
    }
    /// <summary>
    /// 是否有效，1有效，0无效
    /// </summary>
    public enum IsValidType
    {
        有效 = 1,
        无效 = 0
    }
    /// <summary>
    /// 是否存在，1是，0否
    /// </summary>
    public enum IsExistType
    {
        是 = 1,
        否 = 0,
    }
    /// <summary>
    /// 是否需要，1需要，0无
    /// </summary>
    public enum IsNeedType
    {
        需要 = 1,
        不需要 = 0
    }

    /// <summary>
    /// 错误码分类
    /// </summary>
    public enum ErrorCodeType
    {
        本地MES = 1,
        厂级MES = 2,
    }
}