using Automatic.Protocols.HeatingFilmPressurize.Middlewares;
using Automatic.Protocols.HeatingFilmPressurize.Middlewares.Common;
using Automatic.Protocols.HeatingFilmPressurize.Middlewares.Common.PublishNotification;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Automatic.Protocols.HeatingFilmPressurize
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHeatingFilmPressurizeServices(this IServiceCollection services)
        {
            services.AddHostedService<PlcHostedService>();
            services.AddSingleton<HeatingFilmPressurizeFlusher>();
            services.AddSingleton<HeatingFilmPressurizeScanner>();
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
