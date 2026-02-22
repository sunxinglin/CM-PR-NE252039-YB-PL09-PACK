namespace Yee.Entitys.DTOS.AutomaticStationDTOS
{
    public class GlueDataDto
    {
        public int Limit { get; set; } = 20;
        public int Page { get; set; } = 1;
        public string? PackCode { get; set; } = "";
        public string? StationCode { get; set; } = "";
        public string GlueTime { get; set; } = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        public IList<GlueData>? GlueDatas { get; set; }
        public bool IsNeedChangeStatus { get; set; } = true;
    }

    public class GlueData
    {
        public float GlueA { get; set; }
        public float GlueB { get; set; }
        public float GlueProportion { get; set; }
        public float GlueTotal { get; set; }
    }
}
