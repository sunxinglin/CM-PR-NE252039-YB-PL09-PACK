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
    public bool ScrewLayoutEnable { get; set; }
    public int DayStartTime { get; set; } = 8;
    public int DayEndTime { get; set; } = 20;
    public string StationCodeBase { get; set; }
    public bool IsNeedBindOuterCode { get; set; }
    public bool IsNeedBind { get; set; }
    public bool IsNeedUnBind { get; set; }
    public bool RepairBoltFlipX { get; set; }
    public bool RepairBoltFlipY { get; set; }
    public int OverrunAlarmStartSecond { get; set; }
    public int ReversePset { get; set; } = 46;
    public bool IsNeedAutoNC { get; set; } = false;
    public int NGUpTimes { get; set; } = 3;
}
