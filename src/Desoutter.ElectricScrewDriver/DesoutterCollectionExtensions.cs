using Desoutter.ElectricScrewDriver;
using Desoutter.ElectricScrewDriver.BackGroundService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Moxa.IOController
{
    public static class DesoutterServiceCollectionExtensions
    {
        public static IServiceCollection AddDesoutterServices(this IServiceCollection services, IConfiguration desoutterConfig)
        {
            services.AddHostedService<DesoutterBackgroundService>();
            services.AddSingleton<Util>();
            services.AddSingleton<SendMessage>();
            services.AddSingleton<RecvMessage>();
            services.AddSingleton<IDesoutterApi, DesoutterApi>();

            services.AddSingleton<DesoutterMgr>();

            foreach (var config in desoutterConfig.GetChildren())
            {
                services.AddOptions<DesoutterItemOption>(config.Key).Bind(config);
            }

                return services;
        }
    }
}
