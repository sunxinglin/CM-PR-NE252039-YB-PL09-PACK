using Ctp0600P.Shared.CatlMes;

namespace Yee.Services.CatlMesInvoker
{
    public interface ICatlResourceProvider
    {
        /// <summary>
        /// 进站
        /// </summary>
        /// <param name="sfcCode"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public Task<string> GetResourceMIFindCustomAndSfcDataAsync(string sfcCode, MIFindCustomAndSfcDataConfig config);
        public Task<string> GetResourceDataCollectForResourceFAIAsync(DataCollectForResourceFAIConfig config);
    }

    public class DefaultCheckInveotryResourceProvider : ICatlResourceProvider
    {
        public async Task<string> GetResourceMIFindCustomAndSfcDataAsync(string sfcCode, MIFindCustomAndSfcDataConfig config)
        {
            var stdRes = config.InterfaceParams.Resource;
            return stdRes;
        }

        public async Task<string> GetResourceDataCollectForResourceFAIAsync(DataCollectForResourceFAIConfig config)
        {
            var stdRes = config.InterfaceParams.Resource;
            return stdRes;
        }

    }
}
