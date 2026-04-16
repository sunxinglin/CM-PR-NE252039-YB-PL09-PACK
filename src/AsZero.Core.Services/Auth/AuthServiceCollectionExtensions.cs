using AsZero.Core.Services.Repos;
using AsZero.Core.Services.Sys_Logs;

using Microsoft.Extensions.DependencyInjection;

namespace AsZero.Core.Services.Auth
{
    public static class AuthServiceCollectionExtensions
    {
        public static IServiceCollection AddAuth(this IServiceCollection services)
        {
            services.AddSingleton<IPasswordHasher, DefaultPasswordHasher>();
            services.AddScoped<IUserManager, DefaultUserManager>();
            services.AddScoped<FuncModuleService>();
            services.AddScoped<RolesService>();
            services.AddScoped<SysLogService>();
            services.AddScoped<ResourceService>();
            return services;
        }
    }
}


