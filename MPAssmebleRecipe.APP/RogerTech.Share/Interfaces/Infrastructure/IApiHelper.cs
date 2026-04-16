using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RogerTech.Share.Interfaces.Infrastructure
{
    /// <summary>
    /// API帮助类接口
    /// </summary>
    public interface IApiHelper
    {
        /// <summary>
        /// RMS服务客户端
        /// </summary>
        IApiClient RMS { get; }

        /// <summary>
        /// 星云(XY)服务客户端
        /// </summary>
        IApiClient XY { get; }

        /// <summary>
        /// 创建自定义客户端
        /// </summary>
        /// <param name="baseUrl">基础地址</param>
        /// <param name="clientName">客户端名称</param>
        /// <param name="timeout">超时时间（毫秒）</param>
        /// <param name="maxRetryCount">最大重试次数</param>
        /// <returns>API客户端实例</returns>
        IApiClient Create(string baseUrl, string clientName = "Custom", int? timeout = null, int? maxRetryCount = null);

        /// <summary>
        /// 获取RMS基础URL
        /// </summary>
        string RmsBaseUrl { get; }

        /// <summary>
        /// 获取XY基础URL
        /// </summary>
        string XYBaseUrl { get; }

        /// <summary>
        /// 获取默认超时时间（毫秒）
        /// </summary>
        int Timeout { get; }

        /// <summary>
        /// 获取默认最大重试次数
        /// </summary>
        int MaxRetryCount { get; }
    }

    /// <summary>
    /// API客户端接口
    /// </summary>
    public interface IApiClient
    {
        /// <summary>
        /// 发送POST请求
        /// </summary>
        /// <typeparam name="TRequest">请求类型</typeparam>
        /// <typeparam name="TResponse">响应类型</typeparam>
        /// <param name="endpoint">API端点</param>
        /// <param name="requestDto">请求数据</param>
        /// <param name="retryCount">当前重试次数</param>
        /// <returns>响应数据</returns>
        Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest requestDto, int retryCount = 0);

        /// <summary>
        /// 发送POST请求（支持List请求）
        /// </summary>
        Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, List<TRequest> requestDtos, int retryCount = 0);

        /// <summary>
        /// 发送GET请求
        /// </summary>
        /// <typeparam name="TResponse">响应类型</typeparam>
        /// <param name="endpoint">API端点</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="retryCount">当前重试次数</param>
        /// <returns>响应数据</returns>
        Task<TResponse> GetAsync<TResponse>(string endpoint, Dictionary<string, string> parameters = null, int retryCount = 0);

        /// <summary>
        /// 获取基础URL
        /// </summary>
        string BaseUrl { get; }

        /// <summary>
        /// 获取客户端名称
        /// </summary>
        string ClientName { get; }

        /// <summary>
        /// 获取超时时间（毫秒）
        /// </summary>
        int Timeout { get; }

        /// <summary>
        /// 获取最大重试次数
        /// </summary>
        int MaxRetryCount { get; }
    }
}
