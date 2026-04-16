using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TimedTask.ClearHisData;

namespace TimedTask
{
    public static class HistoryDataClearCollectionExtensions
    {
        public static IServiceCollection AddHistoryDataClearServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddOptions<TimedTaskConifg>().Bind(config.GetSection("TimedTaskConifg"));
            services.AddHostedService<ClearHisDataService>();
            return services;
        }
    }
}
