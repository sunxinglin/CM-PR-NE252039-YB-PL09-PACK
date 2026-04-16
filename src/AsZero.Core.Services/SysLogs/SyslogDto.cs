using AsZero.Core.Entities;

namespace AsZero.Core.Services.Sys_Logs
{
    public class SyslogDto
    {
        public Sys_LogType Type { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }
    }
}
