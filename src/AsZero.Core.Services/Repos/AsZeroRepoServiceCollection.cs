using FutureTech.Dal;
using FutureTech.Dal.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace AsZero.Core.Services.Repos
{
    public static class AsZeroRepoServiceCollection
    {
        public static IServiceCollection AddAsZeroRepositories(this IServiceCollection services)
        {
            services.AddDal(o => {
                o.EnableOpsHisotry = false;
            });
            services.AddScoped(typeof(IGenericRepository<,>), typeof(AsZeroEfRepo<,>));
            return services;
        }
    }

}
