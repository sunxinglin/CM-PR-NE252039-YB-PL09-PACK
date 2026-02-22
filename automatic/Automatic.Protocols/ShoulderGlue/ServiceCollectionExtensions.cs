using Automatic.Protocols.ShoulderGlue.Middlewares;
using Automatic.Protocols.ShoulderGlue.Middlewares.Common;
using Automatic.Protocols.ShoulderGlue.Middlewares.Common.PublishNotification;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Automatic.Protocols.ShoulderGlue
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddShoulderGlueServices(this IServiceCollection services)
        {
            // background services & plc processor
            services.AddHostedService<PlcHostedService>();
            services.AddSingleton<ShoulderGlueFlusher>();
            services.AddSingleton<ShoulderGlueScanner>();
            services.AddSingleton<ScanProcessor>();

            services.TryAddScoped<HeartBeatMiddleware>();
            services.AddScoped<PublishNotificationMiddleware>();
            services.AddScoped<FlushPendingMiddleware>();

            services.AddScoped<DealReqEnterStationMiddleware>();
            services.AddScoped<DealReqStartGlueMiddleware>();
            services.AddScoped<DealReqGlueCompleteMiddleware>();
            services.AddScoped<DealReqFirstArticleMiddleware>();

            return services;
        }
    }
}
