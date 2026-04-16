using Ctp0600P.Client.PLC.PLC01.Middlewares;
using Ctp0600P.Client.PLC.PLC01.Middlewares.Common;
using Ctp0600P.Client.PLC.PLC01.Middlewares.Common.PublishNotification;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ctp0600P.Client.PLC.PLC01;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPLC01Services(this IServiceCollection services)
    {
        services.AddHostedService<PlcHostedService>();
        services.AddSingleton<PlcCtrlFlusher>();
        services.AddSingleton<PlcScanner>();
        services.AddSingleton<ScanProcessor>();

        services.TryAddScoped<HeartBeatMiddleware>();
        services.AddScoped<PublishNotificationMiddleware>();
        services.AddScoped<FlushPendingMiddleware>();

        services.AddScoped<DealClientInitialReqMiddleware>();
        services.AddScoped<DealClientAGVBindPackMiddleware>();
        services.AddScoped<DealClientGeneralStatusMiddleware>();
        services.AddScoped<DealClientReleaseAGVMiddleware>();
        services.AddScoped<DealClientTightenStartMiddleware>();
        services.AddScoped<DealPLCAGVArriveMiddleware>();
        services.AddScoped<DealPLCAGVLeaveMiddleware>();
        services.AddScoped<DealPLCAlarmResetMiddleware>();
        services.AddScoped<DealPLCLetGoMiddleware>();
        services.AddScoped<DealPLCModeMiddleware>();
        services.AddScoped<DealPLCTightenCompleteMiddleware>();

        services.AddScoped<DealClientLeakStartMiddware>();
        services.AddScoped<DealClientLeakCompleteMiddleware>();

        return services;
    }
}