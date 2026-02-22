namespace Automatic.Entity
{
    public class ServiceErrResponse
    {
        public bool IsError { get; set; } = true;

        public string ErrorType { get; set; } = string.Empty;
        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class ReponseErrorType
    {
        public const string 上位机错误 = "PCError";
        public const string 同值报警 = "SameValueError";
        public const string CatlMes错误 = "CatlMesError";
        public const string 流程顺序错误 = "FlowError";
        public const string 载具绑定错误 = "VectorBindError";
        public const string 数据异常 = "DataError";
    }
}
