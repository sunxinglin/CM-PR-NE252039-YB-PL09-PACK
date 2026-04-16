using Microsoft.Extensions.DependencyInjection;

namespace Ctp0600P.Client.Apis
{
    public static class ApiServiceCollectionExtensions
    {
        public static void AddClientApis(this IServiceCollection services)
        {
            services.AddSingleton<APIHelper>();
            services.AddSingleton<CatlMesIniConfigApi>();
        }
    }
}
