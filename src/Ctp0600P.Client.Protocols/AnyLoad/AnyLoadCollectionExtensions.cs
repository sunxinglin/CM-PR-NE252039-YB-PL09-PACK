using Ctp0600P.Client.Protocols.AnyLoad;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ctp0600P.Client.Protocols.AnyLoad
{
    public static class AnyLoadServiceCollectionExtensions
    {
        public static IServiceCollection AddAnyLoadServices(this IServiceCollection services)
        {
            services.AddSingleton<AnyLoadMgr>();
            services.AddSingleton<AnyLoadSendMessage>();
            services.AddSingleton<IAnyLoadApi, AnyLoadApi>();
            services.AddHostedService<AnyLoadService>();
            return services;
        }
    }
}
