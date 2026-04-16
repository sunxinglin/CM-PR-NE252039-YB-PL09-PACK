namespace Yee.Entitys.DTOS
{
    public class PressureInfosDto
    {

        public int Limit { get; set; }
        public int Page { get; set; }
        public string? PackCode {get; set; }
        public string? StationCode {get; set; }

        //public DateTime? BeginTime { get; set; }
        //public DateTime? EndTime { get; set; }

    }
}
