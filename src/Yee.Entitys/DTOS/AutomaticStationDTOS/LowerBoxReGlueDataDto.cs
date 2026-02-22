namespace Yee.Entitys.DTOS.AutomaticStationDTOS
{
    public class LowerBoxReGlueDataDto
    {
        public string PackCode { get; set; } = string.Empty;
        public string StationCode { get; set; } = string.Empty;
        public string GlueStartTime { get; set; } = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
    }
}
