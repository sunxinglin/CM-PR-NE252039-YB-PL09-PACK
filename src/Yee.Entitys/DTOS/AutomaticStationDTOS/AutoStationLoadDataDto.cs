namespace Yee.Entitys.DTOS.AutomaticStationDTOS
{
    public class ModuleInBoxDataDto
    {
        public string? PackCode { get; set; } = string.Empty;
        public string? StationCode { get; set; } = string.Empty;

        public int Page { get; set; } = 1;
        public int Limited { get; set; } = 20;
    }

}
