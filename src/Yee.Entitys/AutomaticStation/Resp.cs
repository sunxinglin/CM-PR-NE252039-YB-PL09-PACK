namespace Yee.Entitys.AutomaticStation
{
    public class Resp<TData>
    {
        public bool Success { get; set; }

        public TData? Data { get; set; }

        public ErrorInfo? ErrorInfo { get; set; }

        public string ErrorCode => ErrorInfo?.Code ?? string.Empty;

        public string ErrorMessage => ErrorInfo?.Message ?? string.Empty;
    }

    public class Resp : Resp<object> { }

}
