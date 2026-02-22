using Automatic.Protocols.PressureStripPressurize.Middlewares;
using Automatic.Protocols.PressureStripPressurize.Middlewares.Common;
using Automatic.Protocols.PressureStripPressurize.Middlewares.Common.PublishNotification;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Automatic.Protocols.PressureStripPressurize
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPressureStripServices(this IServiceCollection services)
        {
            services.AddHostedService<PlcHostedService>();
            services.AddSingleton<PressureStripFlusher>();
            services.AddSingleton<PressureStripScanner>();
            services.AddSingleton<ScanProcessor>();

            services.TryAddScoped<HeartBeatMiddleware>();
            services.AddScoped<PublishNotificationMiddleware>();
            services.AddScoped<FlushPendingMiddleware>();

            services.AddScoped<DealReqEnterStationMiddleware>();
            services.AddScoped<DealReqStartPressurizeMiddleware>();
            services.AddScoped<DealReqPressurizeCompleteMiddleware>();

            return services;
        }
    }
}
