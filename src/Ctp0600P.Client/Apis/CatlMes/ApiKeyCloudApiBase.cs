using Ctp0600P.Shared;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ctp0600P.Client.Apis
{
    public class ApiKeyCloudApiBase
    {
        public virtual IOptionsMonitor<ApiServerSetting> CloudSettingMonitor { get; }

        public ApiKeyCloudApiBase(IOptionsMonitor<ApiServerSetting> cloudSettingMonitor)
        {
            this.CloudSettingMonitor = cloudSettingMonitor;
        }

        public virtual ApiServerSetting ApiServerSetting => this.CloudSettingMonitor.CurrentValue;


        /// <summary>
        /// 默认使用 ApiKey 认证
        /// </summary>
        /// <returns></returns>
        protected virtual string CreateApiKeyCredential()
        {
            var settings = this.CloudSettingMonitor.CurrentValue;
            var credentials = this.CreateApiKeyCredential(settings.ApiKeyIdentifier, settings.ApiKey);
            return credentials;
        }

        /// <summary>
        /// 默认使用 ApiKey 认证
        /// </summary>
        /// <returns></returns>
        public virtual string CreateApiKeyCredential(string iden, string key)
        {
            var settings = this.CloudSettingMonitor.CurrentValue;
            var credentials = $"{settings.ApiKeyIdentifier}:{settings.ApiKey}";
            var bytes = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
            var header = $"{settings.ApiKeyPrefix} {bytes}";
            return header;
        }


        /// <summary>
        /// 构建到API 的URL
        /// </summary>
        /// <param name="apiEndPoint">API EndPoint的相对地址，比如"api/online/ping"</param>
        /// <returns></returns>
        protected virtual Uri BuildEndpointUri(string apiEndPoint)
        {
            var settings = this.CloudSettingMonitor.CurrentValue;
            if (string.IsNullOrEmpty(settings.BaseUrl))
            {
                throw new Exception($"{nameof(settings.BaseUrl)} 不可为空！");
            }
            var baseUri = new Uri(settings.BaseUrl);
            var relative = new Uri(apiEndPoint, UriKind.Relative);
            var uri = new Uri(baseUri, relative);
            return uri;
        }

        /// <summary>
        /// 添加Cookie认证
        /// </summary>
        /// <returns></returns>
        protected virtual bool AddCookiesCredential(HttpRequestMessage req, IReadOnlyList<string> cookies)
        {
            if (cookies == null)
            {
                return false;
            }
            foreach (var c in cookies)
            {
                req.Headers.Add("Cookie", c);
            }
            return true;
        }
    }
}
