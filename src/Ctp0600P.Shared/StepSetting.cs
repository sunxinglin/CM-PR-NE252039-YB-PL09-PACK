using Yee.Common.Library.CommonEnum;
using Yee.Entitys.Production;

namespace Ctp0600P.Shared;

public class StepStationSetting
{
    public string Project { get; set; }
    public Base_Step Step { get; set; }
    public Base_Station Station { get; set; }
    public string StationCode { get; set; }
    public string StationName { get; set; }
    public StepTypeEnum StepType { get; set; }
    public bool IsDebug { get; set; }
    /// <summary>
    /// 启用补拧布局加载
    /// </summary>
    public bool ScrewLayoutEnable { get; set; }
    public int DayStartTime { get; set; } = 8;
    public int DayEndTime { get; set; } = 20;
    public bool IsNeedBindOuterCode { get; set; }
    public bool IsNeedBind { get; set; }
    public bool IsNeedUnBind { get; set; }
    /// <summary>
    /// 补拧页面翻转 X轴 / Y轴
    /// </summary>
    public bool RepairBoltFlipX { get; set; }
    public bool RepairBoltFlipY { get; set; }
    /// <summary>
    /// 超时报警秒数
    /// </summary>
    public int OverrunAlarmStartSecond { get; set; }
    public int ReversePset { get; set; } = 46;
    public bool IsNeedAutoNC { get; set; } = false;
    public int NGUpTimes { get; set; } = 3;

    /// <summary>
    /// 窗口全屏时是否遮挡任务栏（默认true遮挡任务栏）
    /// </summary>
    public bool OccupyTaskbar { get; set; } = true;

    /// <summary>
    /// 补拧页面单独缩放比例（默认1.0）
    /// </summary>
    public double RepairPageScale { get; set; } = 1.0;

    /// <summary>
    /// 图示拧紧页面单独缩放比例（默认1.0）
    /// </summary>
    public double TightenByImagePageScale { get; set; } = 1.0;
}

/// <summary>
/// 每种任务类型页面的整体大小缩放配置
/// </summary>
public class PageScaleConfig
{
    public double ScanAccountCard { get; set; } = 1.0;

    public double ScanCode { get; set; } = 1.0;

    public double ScanCollect { get; set; } = 1.0;

    public double BoltGun { get; set; } = 1.0;

    public double AnyLoad { get; set; } = 1.0;
}
