namespace Yee.Entitys.Request
{
    public class AlarmGetPageListRequest : PageBaseRequest
    {
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
