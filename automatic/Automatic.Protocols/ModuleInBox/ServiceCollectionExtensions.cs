using Automatic.Protocols.ModuleInBox.Middlewares;
using Automatic.Protocols.ModuleInBox.Middlewares.Common;
using Automatic.Protocols.ModuleInBox.Middlewares.Common.PublishNotification;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Automatic.Protocols.ModuleInBox
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddModuleInBoxServices(this IServiceCollection services)
        {
            // background services & plc processor
            services.AddHostedService<PlcHostedService>();
            services.AddSingleton<ModuleInBoxFlusher>();
            services.AddSingleton<ModuleInBoxScanner>();
            services.AddSingleton<ScanProcessor>();

            services.TryAddScoped<HeartBeatMiddleware>();
            services.AddScoped<PublishNotificationMiddleware>();
            services.AddScoped<FlushPendingMiddleware>();

            services.AddScoped<DealReqEnterStationMiddleware>();
            services.AddScoped<DealReqTakePhotoCompleteMiddleware>();
            services.AddScoped<DealReqStartInBoxMiddleware>();
            services.AddScoped<DealReqSingleInBoxCompleteMiddleware>();
            services.AddScoped<DealReqInBoxCompleteMiddleware>();
            //services.AddScoped<DealReqTakePhotoMiddleware>();
            //services.AddScoped<DealReqPurificationBlockMiddleware>();
            //services.AddScoped<DealReqPurificationBlockCompleteMiddleware>();

            return services;
        }
    }
}
