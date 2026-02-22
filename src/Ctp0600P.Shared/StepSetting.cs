using Yee.Common.Library.CommonEnum;
using Yee.Entitys.Production;

namespace Ctp0600P.Shared
{
    public class StepStationSetting
    {
        public string Project { get; set; }

        public Base_Step Step { get; set; }
        public Base_Station Station { get; set; }
        public string StationCode { get; set; }
        public StepTypeEnum StepType { get; set; }
        public bool IsDebug { get; set; }
        public bool ScrewLayoutEnable { get; set; }
        public int DayStartTime { get; set; } = 8;
        public int DayEndTime { get; set; } = 20;


        public bool IsNeedBindOuterCode { get; set; } = false;
        public bool IsNeedBind { get; set; } = false;
        public bool IsNeedUnBind { get; set; } = false;
        public bool RepairBoltFlipX { get; set; } = false;
        public bool RepairBoltFlipY { get; set; } = false;
        public int OverrunAlarmStartSecond { get; set; } = 0;
    }
}
