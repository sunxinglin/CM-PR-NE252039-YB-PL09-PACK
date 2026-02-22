using Ctp0600P.Client.PLC.Context;
using Ctp0600P.Client.PLC.PLC01;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StdUnit.Sharp7.Options;

namespace Ctp0600P.Client.PLC
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加Plc相关服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddManualStationPLCServices(this IServiceCollection services, IConfiguration plcItemsConfig, IConfiguration scansConfig)
        {
            services.AddS7PlcOptions(plcItemsConfig, scansConfig);
            services.AddPLC01Services();

            services.AddSingleton<StationPLCContext>();

            return services;
        }
    }
}
