using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RogerTech.Share.Interfaces.Infrastructure;

namespace RogerTech.Tool
{
    /// <summary>
    /// API客户端 - 支持实例化，每个实例可配置独立的基础地址
    /// 实现IApiClient接口，支持依赖注入
    /// </summary>
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly int _timeout;
        private readonly int _maxRetryCount;
        private readonly string _clientName;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="baseUrl">API基础地址</param>
        /// <param name="clientName">客户端名称（用于日志标识）</param>
        /// <param name="timeout">超时时间（毫秒），默认30000</param>
        /// <param name="maxRetryCount">最大重试次数，默认3</param>
        public ApiClient(string baseUrl, string clientName = "Default", int timeout = 30000, int maxRetryCount = 3)
        {
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentException("baseUrl不能为空");

            _baseUrl = baseUrl.TrimEnd('/');
            _clientName = clientName;
            _timeout = timeout;
            _maxRetryCount = maxRetryCount;

            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMilliseconds(_timeout)
            };
        }

        #region POST请求

        /// <summary>
        /// 发送POST请求
        /// </summary>
        /// <typeparam name="TRequest">请求类型</typeparam>
        /// <typeparam name="TResponse">响应类型</typeparam>
        /// <param name="endpoint">API端点</param>
        /// <param name="requestDto">请求数据</param>
        /// <param name="retryCount">当前重试次数</param>
        /// <returns>响应数据</returns>
        public async Task<TResponse> PostAsync<TRequest, TResponse>(
            string endpoint,
            TRequest requestDto,
            int retryCount = 0)
        {
            var url = CombineUrl(endpoint);
            var startTime = DateTime.Now;

            try
            {
                LogInfo($"[{_clientName}] [POST] 请求开始: {url}");

                var jsonContent = JsonConvert.SerializeObject(requestDto, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatString = "yyyy-MM-dd HH:mm:ss"
                });

                LogInfo($"[{_clientName}] [POST] 请求内容: {jsonContent}");

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);
                var responseJson = await response.Content.ReadAsStringAsync();

                LogInfo($"[{_clientName}] [POST] 响应内容: {responseJson}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"HTTP错误: {response.StatusCode}, 内容: {responseJson}");
                }

                var result = JsonConvert.DeserializeObject<TResponse>(responseJson);
                var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
                LogInfo($"[{_clientName}] [POST] 请求成功，耗时: {elapsed}ms");

                return result;
            }
            catch (Exception ex)
            {
                var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
                LogError($"[{_clientName}] [POST] 请求失败，耗时: {elapsed}ms, 错误: {ex.Message}");

                if (retryCount < _maxRetryCount)
                {
                    LogWarning($"[{_clientName}] [POST] 开始第 {retryCount + 1} 次重试...");
                    await Task.Delay(1000 * (retryCount + 1));
                    return await PostAsync<TRequest, TResponse>(endpoint, requestDto, retryCount + 1);
                }

                LogError($"[{_clientName}] [POST] 重试 {_maxRetryCount} 次后仍然失败");
                throw;
            }
        }

        /// <summary>
        /// 发送POST请求（支持List请求）
        /// </summary>
        /// <typeparam name="TRequest">请求类型</typeparam>
        /// <typeparam name="TResponse">响应类型</typeparam>
        /// <param name="endpoint">API端点</param>
        /// <param name="requestDtos">请求数据列表</param>
        /// <param name="retryCount">当前重试次数</param>
        /// <returns>响应数据</returns>
        public async Task<TResponse> PostAsync<TRequest, TResponse>(
            string endpoint,
            List<TRequest> requestDtos,
            int retryCount = 0)
        {
            return await PostAsync<List<TRequest>, TResponse>(endpoint, requestDtos, retryCount);
        }

        #endregion

        #region GET请求

        /// <summary>
        /// 发送GET请求
        /// </summary>
        /// <typeparam name="TResponse">响应类型</typeparam>
        /// <param name="endpoint">API端点</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="retryCount">当前重试次数</param>
        /// <returns>响应数据</returns>
        public async Task<TResponse> GetAsync<TResponse>(
            string endpoint,
            Dictionary<string, string> parameters = null,
            int retryCount = 0)
        {
            var url = CombineUrl(endpoint);
            if (parameters != null && parameters.Count > 0)
            {
                url += "?" + BuildQueryString(parameters);
            }

            var startTime = DateTime.Now;

            try
            {
                LogInfo($"[{_clientName}] [GET] 请求开始: {url}");

                var response = await _httpClient.GetAsync(url);
                var responseJson = await response.Content.ReadAsStringAsync();

                LogInfo($"[{_clientName}] [GET] 响应内容: {responseJson}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"HTTP错误: {response.StatusCode}, 内容: {responseJson}");
                }

                var result = JsonConvert.DeserializeObject<TResponse>(responseJson);
                var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
                LogInfo($"[{_clientName}] [GET] 请求成功，耗时: {elapsed}ms");

                return result;
            }
            catch (Exception ex)
            {
                var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
                LogError($"[{_clientName}] [GET] 请求失败，耗时: {elapsed}ms, 错误: {ex.Message}");

                if (retryCount < _maxRetryCount)
                {
                    LogWarning($"[{_clientName}] [GET] 开始第 {retryCount + 1} 次重试...");
                    await Task.Delay(1000 * (retryCount + 1));
                    return await GetAsync<TResponse>(endpoint, parameters, retryCount + 1);
                }

                LogError($"[{_clientName}] [GET] 重试 {_maxRetryCount} 次后仍然失败");
                throw;
            }
        }

        #endregion

        #region 辅助方法

        private string CombineUrl(string endpoint)
        {
            endpoint = endpoint.TrimStart('/');
            return $"{_baseUrl}/{endpoint}";
        }

        private static string BuildQueryString(Dictionary<string, string> parameters)
        {
            var queryString = new StringBuilder();
            foreach (var param in parameters)
            {
                if (queryString.Length > 0)
                    queryString.Append("&");
                queryString.Append($"{Uri.EscapeDataString(param.Key)}={Uri.EscapeDataString(param.Value ?? "")}");
            }
            return queryString.ToString();
        }

        #endregion

        #region 日志方法

        private static void LogInfo(string message)
        {
            var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [INFO] {message}";
            Console.WriteLine(logMessage);
            System.Diagnostics.Debug.WriteLine(logMessage);
        }

        private static void LogWarning(string message)
        {
            var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [WARN] {message}";
            Console.WriteLine(logMessage);
            System.Diagnostics.Debug.WriteLine(logMessage);
        }

        private static void LogError(string message)
        {
            var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [ERROR] {message}";
            Console.WriteLine(logMessage);
            System.Diagnostics.Debug.WriteLine(logMessage);
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取基础URL
        /// </summary>
        public string BaseUrl => _baseUrl;

        /// <summary>
        /// 获取客户端名称
        /// </summary>
        public string ClientName => _clientName;

        /// <summary>
        /// 获取超时时间（毫秒）
        /// </summary>
        public int Timeout => _timeout;

        /// <summary>
        /// 获取最大重试次数
        /// </summary>
        public int MaxRetryCount => _maxRetryCount;

        #endregion
    }
}
