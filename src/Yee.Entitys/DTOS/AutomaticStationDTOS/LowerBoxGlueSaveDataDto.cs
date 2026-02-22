namespace Yee.Entitys.DTOS.AutomaticStationDTOS
{
    public class LowerBoxGlueSaveDataDto
    {
        public string PackCode { get; set; } = string.Empty;
        public string StationCode { get; set; } = string.Empty;
        public string GlueStartTime { get; set; } = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        public Dictionary<string, string>? GlueDatas { get; set; }
        public bool IsNeedChangeStatus { get; set; } = true;
    }

    public class LowBoxGlueData
    {
        public float GlueA { get; set; }
        public float GlueB { get; set; }
        public float GlueProportion { get; set; }
        public float GlueTotal { get; set; }
    }
}
