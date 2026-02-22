using Automatic.Protocols.LowerBoxGlue.Middlewares;
using Automatic.Protocols.LowerBoxGlue.Middlewares.Common;
using Automatic.Protocols.LowerBoxGlue.Middlewares.Common.PublishNotification;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Automatic.Protocols.LowerBoxGlue
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLowerBoxGlueServices(this IServiceCollection services)
        {
            services.AddHostedService<PlcHostedService>();
            services.AddSingleton<LowerBoxGlueFlusher>();
            services.AddSingleton<LowerBoxGlueScanner>();
            services.AddSingleton<ScanProcessor>();

            services.TryAddScoped<HeartBeatMiddleware>();
            services.AddScoped<PublishNotificationMiddleware>();
            services.AddScoped<FlushPendingMiddleware>();

            services.AddScoped<DealReqEnterStationMiddleware>();
            services.AddScoped<DealReqStartGlueMiddleware>();
            services.AddScoped<DealReqGlueCompleteMiddleware>();
            services.AddScoped<DealReqReGlueStartMiddleware>();
            services.AddScoped<DealReqReGlueCompleteMiddleware>();
            services.AddScoped<DealReqFirstArticleMiddleware>();

            return services;
        }
    }
}
