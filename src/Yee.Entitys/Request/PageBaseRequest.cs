namespace Yee.Entitys.Request
{
    public class PageBaseRequest
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
    }
}
