namespace Automatic.Shared
{
    public class ApiServerSetting
    {
        /// <summary>
        /// cloud BaseURL
        /// </summary>
        public string BaseUrl { get; set; } = "";

        public string ApiKeyPrefix { get; set; } = "itminus.Key";

        public string ApiKeyIdentifier { get; set; } = "";

        /// <summary>
        /// Api Key to Access Cloud Resource
        /// </summary>
        public string ApiKey { get; set; } = "";

        public string DefaultOperatorAccount { get; set; } = "Operator";
        public string DefaultOperatorPassword { get; set; } = "123456";

        public string CheckSfc { get; set; } = "";
        public bool IsDebug { get; set; } = true;

        public string StepCode { get; set; } = "";

        public bool IsEmptyLoop { get; set; } = false;


    }
}
