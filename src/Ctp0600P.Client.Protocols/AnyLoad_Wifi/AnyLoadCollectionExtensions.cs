using Microsoft.Extensions.DependencyInjection;

namespace Ctp0600P.Client.Protocols.AnyLoad_Wifi;

public static class AnyLoadServiceCollectionExtensions
{
    public static IServiceCollection AddAnyLoadWifiServices(this IServiceCollection services)
    {
        services.AddSingleton<AnyLoadMgr>();
        services.AddSingleton<AnyLoadSendMessage>();
        services.AddSingleton<IAnyLoadApi, AnyLoadApi>();
        services.AddHostedService<AnyLoadService>();
        return services;
    }
}