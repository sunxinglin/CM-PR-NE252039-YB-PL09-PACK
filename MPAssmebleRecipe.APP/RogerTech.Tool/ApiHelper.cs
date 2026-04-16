using System;
using System.Configuration;
using RogerTech.Share.Interfaces.Infrastructure;

namespace RogerTech.Tool
{
    /// <summary>
    /// API通讯帮助类 - 支持依赖注入
    /// </summary>
    public class ApiHelper : IApiHelper
    {
        private readonly ApiClient _rmsClient;
        private readonly ApiClient _xyClient;
        private readonly string _rmsBaseUrl;
        private readonly string _xyBaseUrl;
        private readonly int _timeout;
        private readonly int _maxRetryCount;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ApiHelper()
        {
            // 从配置文件读取
            _rmsBaseUrl = ConfigurationManager.AppSettings["RmsApiBaseUrl"] ?? "http://localhost:8080";
            _xyBaseUrl = ConfigurationManager.AppSettings["XYApiBaseUrl"] ?? "http://localhost:8080";
            _timeout = int.Parse(ConfigurationManager.AppSettings["ApiTimeout"] ?? "30000");
            _maxRetryCount = int.Parse(ConfigurationManager.AppSettings["ApiRetryCount"] ?? "3");

            _rmsClient = new ApiClient(_rmsBaseUrl, "RMS", _timeout, _maxRetryCount);
            _xyClient = new ApiClient(_xyBaseUrl, "XY", _timeout, _maxRetryCount);
        }

        /// <summary>
        /// RMS服务客户端
        /// </summary>
        public IApiClient RMS => _rmsClient;

        /// <summary>
        /// 星云(XY)服务客户端
        /// </summary>
        public IApiClient XY => _xyClient;

        /// <summary>
        /// 创建自定义客户端
        /// </summary>
        public IApiClient Create(string baseUrl, string clientName = "Custom", int? timeout = null, int? maxRetryCount = null)
        {
            return new ApiClient(
                baseUrl,
                clientName,
                timeout ?? _timeout,
                maxRetryCount ?? _maxRetryCount);
        }

        /// <summary>
        /// 获取RMS基础URL
        /// </summary>
        public string RmsBaseUrl => _rmsBaseUrl;

        /// <summary>
        /// 获取XY基础URL
        /// </summary>
        public string XYBaseUrl => _xyBaseUrl;

        /// <summary>
        /// 获取默认超时时间（毫秒）
        /// </summary>
        public int Timeout => _timeout;

        /// <summary>
        /// 获取默认最大重试次数
        /// </summary>
        public int MaxRetryCount => _maxRetryCount;
    }
}
