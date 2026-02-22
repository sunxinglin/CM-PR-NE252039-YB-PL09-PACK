using Automatic.Protocols.UpperCoverTighten.Middlewares;
using Automatic.Protocols.UpperCoverTighten.Middlewares.Common;
using Automatic.Protocols.UpperCoverTighten.Middlewares.Common.PublishNotification;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Automatic.Protocols.UpperCoverTighten
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddUpperCoverTightenServices(this IServiceCollection services)
        {
            // background services & plc processor
            services.AddHostedService<PlcHostedService>();
            services.AddSingleton<UpperCoverTightenFlusher>();
            services.AddSingleton<UpperCoverTightenScanner>();
            services.AddSingleton<ScanProcessor>();

            services.TryAddScoped<HeartBeatMiddleware>();
            services.AddScoped<PublishNotificationMiddleware>();
            services.AddScoped<FlushPendingMiddleware>();

            services.AddScoped<DealReqEnterStationMiddleware>();
            services.AddScoped<DealReqStartTightenMiddleware>();
            services.AddScoped<DealReqTightenCompleteMiddleware>();
            return services;
        }


    }
}
