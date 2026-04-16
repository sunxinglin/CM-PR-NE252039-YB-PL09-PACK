using System.Net.Http;
using System.Threading.Tasks;

using Ctp0600P.Shared;
using Ctp0600P.Shared.CatlMes;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Yee.Services.CatlMesInvoker;

namespace Ctp0600P.Client.Apis
{
    public class CatlMesIniConfigApi : ApiKeyCloudApiBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CatlMesIniConfigApi> _logger;
        private readonly CatlMesIniConfigHelper _catlMesIniConfigHelper;

        public CatlMesIniConfigApi(HttpClient httpClient, ILogger<CatlMesIniConfigApi> logger, CatlMesIniConfigHelper catlMesIniConfigHelper, IOptionsMonitor<ApiServerSetting> cloudSettingMonitor) : base(cloudSettingMonitor)
        {
            this._httpClient = httpClient;
            this._logger = logger;
            this._catlMesIniConfigHelper = catlMesIniConfigHelper;
        }
      
        #region 进站
        public async Task<MIFindCustomAndSfcDataConfig> GetMIFindCustomAndSfcDataConfig(string opCode="")
        {
           return _catlMesIniConfigHelper.GetMIFindCustomAndSfcDataConfig(opCode);
        }

        public async Task<UnitResp> SetMIFindCustomAndSfcDataConfig(MIFindCustomAndSfcDataConfig config,string opCode="")
        {
            _catlMesIniConfigHelper.SetMIFindCustomAndSfcDataConfig(config, opCode);
            return new UnitResp();
        }
        #endregion

        #region 首件
        public async Task<DataCollectForResourceFAIConfig> GetDataCollectForResourceFAIConfig()
        {
            return _catlMesIniConfigHelper.GetDataCollectForResourceFAIConfig();
        }

        public async Task<UnitResp> SetDataCollectForResourceFAIConfig(DataCollectForResourceFAIConfig config)
        {
            _catlMesIniConfigHelper.SetDataCollectForResourceFAIConfig(config);
            return new UnitResp();
        }
        #endregion

        #region 检验
        public async Task<MICheckSFCStatusExConfig> GetMICheckSfcStatusExConfig(string AutoOpName = "")
        {
            return _catlMesIniConfigHelper.GetMICheckSfcStatusExConfig(AutoOpName);
        }

        public async Task<UnitResp> SetMICheckSfcStatusExConfig(MICheckSFCStatusExConfig config, string AutoOpName = "")
        {
            _catlMesIniConfigHelper.SetMICheckSFCStatusExConfig(config, AutoOpName);
            return new UnitResp();
        }
        #endregion

        #region 校验贴纸PN以及库存
        public async Task<MICheckBOMInventoryConfig> GetMICheckBOMInventoryConfig(string AutoOpName = "")
        {
            return _catlMesIniConfigHelper.GetMICheckBOMInventoryConfig(AutoOpName);
        }

        public async Task<UnitResp> SetMICheckBOMInventoryConfig(MICheckBOMInventoryConfig config, string AutoOpName = "")
        {
            _catlMesIniConfigHelper.SetMICheckBOMInventoryConfig(config, AutoOpName);
            return new UnitResp();
        }
        #endregion

        #region 组装
        public async Task<MIAssembleAndCollectDataForSfcConfig> GetMiAssembleAndCollectDataForSfcConfig(string opCode="")
        {
            return _catlMesIniConfigHelper.GetMiAssembleAndCollectDataForSfcConfig(opCode);
        }

        public async Task<UnitResp> SetMiAssembleAndCollectDataForSfcConfig(MIAssembleAndCollectDataForSfcConfig config, string opCode = "")
        {
            _catlMesIniConfigHelper.SetMiAssembleAndCollectDataForSfcConfig(config, opCode);
            return new UnitResp();
        }
        #endregion

        #region 收数
        public async Task<DataCollectForMoudleTestConfig> GetDataCollectForMoudleTestConfig(string opCode = "")
        {
            return _catlMesIniConfigHelper.GetDataCollectForMoudleTestConfig( opCode );
        }

        public async Task<UnitResp> SetDataCollectForMoudleTestConfig(DataCollectForMoudleTestConfig config,string AutoOpName="")
        {
            _catlMesIniConfigHelper.SetDataCollectForMoudleTestConfig(config, AutoOpName);
            return new UnitResp();
        }

        public async Task<FindSfcByInventoryMIFindCustomAndSfcDataConfig> GetFindSfcByInventoryConfig(string opCode = "")
        {
          return _catlMesIniConfigHelper.GetFindSfcByInventoryConfig(opCode);

        }

        public async Task<UnitResp> SetFindSfcByInventoryConfig(FindSfcByInventoryMIFindCustomAndSfcDataConfig config, string AutoOpName = "")
        {
            _catlMesIniConfigHelper.SetFindSfcByInventoryConfig(config, AutoOpName);
            return new UnitResp();


        }



        #endregion
        #region 首件
        public async Task<DataCollectForResourceInspectConfig> GetDataCollectForResourceInspectConfig(string opCode = "")
        {
            return _catlMesIniConfigHelper.GetDataCollectForResourceInspectConfig(opCode);

        }

        public async Task<UnitResp> SetDataCollectForResourceInspectConfig(DataCollectForResourceInspectConfig config, string AutoOpName = "")
        {
            _catlMesIniConfigHelper.SetDataCollectForResourceInspectConfig(config, AutoOpName);
            return new UnitResp();


        }


        #endregion
    }
}
