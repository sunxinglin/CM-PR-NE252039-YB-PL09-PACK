using AsZero.Core.Entities;
using AsZero.Services.Auth;

using Microsoft.AspNetCore.Authentication.Cookies;

namespace Yee.Services.Auth
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddYeeAuth(this IServiceCollection services)
        {

            // 默认采用Cookie认证
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(opts =>
                    {
                        opts.LoginPath = "/login";
                        opts.LogoutPath = "/logout";
                        opts.Events.OnRedirectToAccessDenied = (ctx) =>
                        {
                            if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                            {
                                ctx.Response.StatusCode = 403;
                            }
                            return Task.CompletedTask;
                        };
                    })
                    .AddScheme<ApiKeyAuthOpts, ApiKeyAuthencationHandler>(
                        ApiKeyAuthOpts.DefaultAuthenticationSchemeName,
                        opts => { }
                    )
                    ;
            services.AddAuthorization(opts =>
            {
                // Cookie 及 ApiKey 均可以访问的接口
                opts.AddPolicy(AuthDefines.Policy_CookieAuthOrApiKeyAuth, pb => {
                    pb.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
                      .AddAuthenticationSchemes(ApiKeyAuthOpts.DefaultAuthenticationSchemeName);
                    pb.RequireAuthenticatedUser();
                });
                // 只能使用管理型 ApiKey 访问
                opts.AddPolicy(AuthDefines.Policy_AdminApiKeyAuth, pb =>
                {
                    pb.AddAuthenticationSchemes(ApiKeyAuthOpts.DefaultAuthenticationSchemeName);
                    pb.RequireAuthenticatedUser();
                    pb.RequireClaim(nameof(ApiKeyType), Enum.GetName(typeof(ApiKeyType), ApiKeyType.Admin)!);
                });
                // 只能工站型ApiKey访问
                opts.AddPolicy(AuthDefines.Policy_SubstationApiKeyAuth, pb =>
                {
                    pb.AddAuthenticationSchemes(ApiKeyAuthOpts.DefaultAuthenticationSchemeName);
                    pb.RequireAuthenticatedUser();
                    pb.RequireClaim(nameof(ApiKeyType), Enum.GetName(typeof(ApiKeyType), ApiKeyType.Substation)!);
                });
            });

            return services;
        }
    }

}
