namespace Yee.Services.Request
{
    public class AlarmRequest
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public int StationId { get; set; } = 0;
        public string Description { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public bool IsFinish { get; set; } = false;
        public DateTime OccurTime { get; set; }
        public DateTime ClearDate { get; set; }
    }
    public class AlarmDto
    {
        public string StationCode { get; set; } = string.Empty;
        public int StationId { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }
    }

    public class GetAlarmsByStationRequest
    {
        public string StationName { get; set; }
    }
}
