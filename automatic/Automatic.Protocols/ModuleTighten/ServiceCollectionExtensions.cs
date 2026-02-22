using Automatic.Protocols.ModuleTighten.Middlewares;
using Automatic.Protocols.ModuleTighten.Middlewares.Common;
using Automatic.Protocols.ModuleTighten.Middlewares.Common.PublishNotification;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Automatic.Protocols.ModuleTighten
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddModuleTightenServices(this IServiceCollection services)
        {
            // background services & plc processor
            services.AddHostedService<PlcHostedService>();
            services.AddSingleton<ModuleTightenFlusher>();
            services.AddSingleton<ModuleTightenScanner>();
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
