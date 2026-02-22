using Automatic.Protocols.TerminalReshape.Middlewares;
using Automatic.Protocols.TerminalReshape.Middlewares.Common;
using Automatic.Protocols.TerminalReshape.Middlewares.Common.PublishNotification;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Automatic.Protocols.TerminalReshape
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTerminalReshapeServices(this IServiceCollection services)
        {
            services.AddHostedService<PlcHostedService>();
            services.AddSingleton<TerminalReshapeFlusher>();
            services.AddSingleton<TerminalReshapeScanner>();
            services.AddSingleton<ScanProcessor>();
            services.TryAddScoped<HeartBeatMiddleware>();
            services.AddScoped<PublishNotificationMiddleware>();
            services.AddScoped<FlushPendingMiddleware>();
            services.AddScoped<DealReqVectorEnterMiddleware>();
            services.AddScoped<DealReqStartReshapeMiddleware>();
            services.AddScoped<DealReqCompleteReshapeMiddleware>();

            return services;
        }
    }
}
