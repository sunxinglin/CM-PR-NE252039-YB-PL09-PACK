using AsZero.Core.Services.Repos;

using Ctp0600P.Shared;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using Yee.Entitys.DTOS;

namespace Yee.Services.Helper
{
    public class APIHelper
    {
        private ILogger<APIHelper> _logger;
        protected ApiServerSetting ServerSetting { get; }
        private readonly IConfiguration configuration;

        public APIHelper(ILogger<APIHelper> logger, IOptionsMonitor<ApiServerSetting> cloudSetting, IConfiguration configuration)
        {
            this._logger = logger;
            this.ServerSetting = cloudSetting.CurrentValue;
            this.configuration = configuration;
        }

        public async Task<BindAgvResponse> BindAgv(BindAgvDTO agv)
        {
            var ip = configuration.GetConnectionString("AGVServiceUrl");
            string content = HttpHelper.PostJson($"{ip}/api/AgvMES/MesBind", JsonConvert.SerializeObject(
                 new
                 {
                     AgvCode = agv.AgvCode,
                     ProductCode = agv.PackPN,
                     ProductType = agv.ProductType,
                     Behavior = (int)agv.State,
                     HolderBarCode = string.IsNullOrEmpty(agv.HolderBarCode) ? agv.PackPN : agv.HolderBarCode
                 }
                ));
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }
            var result = JsonConvert.DeserializeObject<BindAgvResponse>(content);
            return result;
        }

        public async Task<BindAgvResponse> BindAgv_JT(BindAgvDTO agv)
        {
            var ip = configuration.GetConnectionString("AGVServiceUrl");
            //string content = HttpHelper.PostJson($"{ip}/IDataInterface/AGVMaterialBindingToAGV", JsonConvert.SerializeObject(
            //     new
            //     {
            //         AGVId = agv.AgvCode,
            //         MaterialCode = agv.State == BingAgvStateEnum.绑定 ? agv.PackPN : null,
            //         MaterialType = agv.State == BingAgvStateEnum.绑定 ? agv.ProductType : null,
            //         HolderBarCode = agv.State == BingAgvStateEnum.绑定 ? agv.HolderBarCode : null,
            //         IsBinding = true
            //     }
            //    ));
            string content = HttpHelper.PostJson($"{ip}/api/AgvMES/MesBind", JsonConvert.SerializeObject(
                 new
                 {
                     AgvCode = agv.AgvCode,
                     ProductCode = agv.PackPN,
                     HolderBarCode =agv.HolderBarCode,
                     ProductType = agv.ProductType,
                     Behavior = (int)agv.State
                 }
                ));
            if (string.IsNullOrEmpty(content)) return null;
            var result = JsonConvert.DeserializeObject<BindAgvResponse>(content);
            if (result.Message == agv.PackPN)
                result.Success = true;
            if(agv.State == BingAgvStateEnum.解绑)
                result.Success = true;
            return result;
        }

        public async Task<BindAgvResponse> RunAgv(string agvCode, int releaseType)
        {
            var ip = configuration.GetConnectionString("AGVServiceUrl");
            string content = HttpHelper.PostJson($"{ip}/api/AgvMES/ReleaseAGV", JsonConvert.SerializeObject(
                 new
                 {
                     AgvCode = agvCode,
                     ReleaseType = releaseType,
                 }
                ));
            if (string.IsNullOrEmpty(content)) return null;

            var result = JsonConvert.DeserializeObject<BindAgvResponse>(content);
            _logger.LogInformation($"放行AGV返回{content}");
            return result;
        }

        public async Task<Response> RunAgv_JT(string agvCode, int releaseType)
        {
            var ip = configuration.GetConnectionString("AGVServiceUrl");
            //string content = HttpHelper.PostJson($"{ip}/IDataInterface/AGVRelease", JsonConvert.SerializeObject(
            //     new
            //     {
            //         AgvId = Convert.ToInt16(agvCode),
            //         ReleaseStatus = Convert.ToInt16(releaseType),
            //     }
            //    ));
            string content = HttpHelper.PostJson($"{ip}/api/AgvMES/ReleaseAGV", JsonConvert.SerializeObject(
                 new
                 {
                     //AgvCode = Convert.ToInt16(agvCode),
                     //ReleaseType = Convert.ToInt16(releaseType),
                     AgvCode = agvCode,
                     ReleaseType = 1,
                     StationCode = releaseType.ToString()
                 }
                ));
            if (string.IsNullOrEmpty(content)) return new Response { Code=500,Message="AGV放行服务返回空，请联系AGV运维"};

            var result = JsonConvert.DeserializeObject<Response>(content);
            _logger.LogInformation($"放行AGV返回{content}");
            return result;
        }
    }
}
