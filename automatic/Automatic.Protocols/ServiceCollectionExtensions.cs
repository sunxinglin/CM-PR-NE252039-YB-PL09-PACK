using Automatic.Protocols.HeatingFilmPressurize;
using Automatic.Protocols.LowerBoxGlue;
using Automatic.Protocols.ModuleInBox;
using Automatic.Protocols.PressureStripPressurize;
using Automatic.Protocols.ShoulderGlue;
using Automatic.Protocols.UpperCoverTighten;
using Automatic.Protocols.UpperCoverTighten2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StdUnit.Sharp7.Options;

namespace Automatic.Protocols
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加Plc相关服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddPlcServices(this IServiceCollection services, IConfiguration plcItemsConfig, IConfiguration scansConfig)
        {
            services.AddS7PlcOptions(plcItemsConfig, scansConfig);
            services.AddHeatingFilmPressurizeServices();
            services.AddLowerBoxGlueServices();
            services.AddModuleInBoxServices();
            services.AddShoulderGlueServices();
            services.AddPressureStripServices();
            services.AddUpperCoverTightenServices();
            services.AddUpperCoverTighten2Services();

            //services.AddModuleTightenServices();
            //services.AddTerminalReshapeServices();
            return services;
        }

    }
}
