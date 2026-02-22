using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HJZK.IOController
{
    public static class IOBoxServiceCollectionExtensions
    {
        public static IServiceCollection AddHJZKIOServices(this IServiceCollection services, IConfiguration zlanConfig)
        {
            foreach (var config in zlanConfig.GetChildren())
            {
                services.AddOptions<IOBoxConfig>(config.Key).Bind(config);
            }

            services.AddSingleton<IIOBoxApi, IOBoxApi>();
            services.AddSingleton<IOMessageNotification>();
            services.AddHostedService<IOBox01_BackgroundService>();
            //services.AddHostedService<IOBox02_BackgroundService>();
            return services;
        }
    }
}
