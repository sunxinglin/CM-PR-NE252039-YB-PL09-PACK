namespace Ctp0600P.Shared
{
    public class ApiServerSetting
    {
        public string AgvUrl { get; set; }

        /// <summary>
        /// cloud BaseURL
        /// </summary>
        public string BaseUrl { get; set; }

        public string ApiKeyPrefix { get; set; } = "itminus.Key";

        /// <summary>
        /// Api Key Identifier
        /// </summary>
        public string ApiKeyIdentifier { get; set; }

        /// <summary>
        /// Api Key to Access Cloud Resource
        /// </summary>
        public string ApiKey { get; set; }
    }


}
