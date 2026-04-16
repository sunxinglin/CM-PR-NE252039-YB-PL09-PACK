namespace Yee.Entitys.AutomaticStation
{
    public class ServiceErrResponse
    {
        public bool IsError { get; set; } = true;

        public string ErrorType { get; set; } = ResponseErrorType.上位机错误;
        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;

        public virtual ServiceErrResponse ToSuccess()
        {
            this.IsError = false;
            this.ErrorType = "";
            return this;
        }

        public virtual ServiceErrResponse ToError(string errorType, int errorCode, string errorMessage)
        {
            this.IsError = true;
            ErrorType = errorType;
            this.ErrorCode = errorCode;
            this.ErrorMessage = errorMessage;
            return this;
        }
    }

    public class ResponseErrorType
    {
        public const string 上位机错误 = "PCError";
        public const string 同值报警 = "SameValueError";
        public const string CatlMes错误 = "CatlMesError";
        public const string 流程顺序错误 = "FlowError";
        public const string 载具绑定错误 = "VectorBindError";
        public const string 数据异常 = "DataError";
        public const string 超限报警 = "超限报警";
    }
}
