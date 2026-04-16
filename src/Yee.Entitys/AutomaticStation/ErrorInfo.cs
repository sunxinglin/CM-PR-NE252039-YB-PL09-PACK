namespace Yee.Entitys.AutomaticStation
{
    public class ErrorInfo
    {
        public string Code { get; set; } = "";


        public string Message { get; set; } = "";


        public IList<ErrorField> ErrorFields { get; set; }

        public static ErrorInfo MakeNew(string code, string message)
        {
            return new ErrorInfo
            {
                Code = code,
                Message = message,
                ErrorFields = new List<ErrorField>()
            };
        }

        public static ErrorInfo MakeNew(string message)
        {
            return new ErrorInfo
            {
                Code = "ERR_MISC",
                Message = message,
                ErrorFields = new List<ErrorField>()
            };
        }
    }
}
