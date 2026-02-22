using Automatic.Protocols.UpperCoverTighten2.Middlewares;
using Automatic.Protocols.UpperCoverTighten2.Middlewares.Common;
using Automatic.Protocols.UpperCoverTighten2.Middlewares.Common.PublishNotification;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Automatic.Protocols.UpperCoverTighten2
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddUpperCoverTighten2Services(this IServiceCollection services)
        {
            // background services & plc processor
            services.AddHostedService<PlcHostedService>();
            services.AddSingleton<PlcFlusher>();
            services.AddSingleton<PlcScanner>();
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
