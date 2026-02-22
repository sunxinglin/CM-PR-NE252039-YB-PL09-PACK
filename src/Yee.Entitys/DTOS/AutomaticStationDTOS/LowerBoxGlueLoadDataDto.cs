namespace Yee.Entitys.DTOS.AutomaticStationDTOS
{
    public class LowerBoxGlueLoadDataDto
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;
        public string? PackCode { get; set; } = string.Empty;
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
