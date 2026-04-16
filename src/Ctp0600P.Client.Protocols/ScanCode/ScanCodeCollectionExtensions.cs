using Ctp0600P.Client.Protocols.ScanCode;

using Microsoft.Extensions.DependencyInjection;

namespace Ctp0600P.Client.Protocols;

public static class ScanCodeServiceCollectionExtensions
{
    public static IServiceCollection AddScanCodeServices(this IServiceCollection services)
    {
        services.AddSingleton<ScanCodeMgr>();
        services.AddHostedService<ScanCodeService>();
        return services;
    }
}