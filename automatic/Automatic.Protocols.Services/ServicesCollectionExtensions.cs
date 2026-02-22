using Microsoft.Extensions.DependencyInjection;

namespace Automatic.Protocols.Services
{
    public static class ServicesCollectionExtensions
    {
        public static IServiceCollection AddDealPlcReqServices(this IServiceCollection services)
        {
            services.AddSingleton<CommonService>();
            services.AddSingleton<AutoTightenService>();
            services.AddSingleton<ModuleInBoxService>();
            services.AddSingleton<AutoGlueService>();
            services.AddSingleton<AutoPressureService>();
            services.AddSingleton<HeatingFilmPressurizeService>();

            services.AddSingleton<LowerBoxGlueService>();
            services.AddSingleton<LowerBoxReGlueService>();

            return services;
        }
    }
}
