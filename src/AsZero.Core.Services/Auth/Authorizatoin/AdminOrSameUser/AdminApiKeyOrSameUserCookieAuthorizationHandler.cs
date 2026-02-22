using AsZero.Core.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AsZero.Services.Auth
{
    internal class AdminApiKeyOrSameUserCookieAuthorizationHandler : AuthorizationHandler<AdminApiKeyOrSameUserCookieAuthorizationRequirement, string>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminApiKeyOrSameUserCookieAuthorizationRequirement requirement, string resource)
        {
            var user = context.User;
            var scheme = user?.Identity?.AuthenticationType;
            if (scheme == ApiKeyAuthOpts.DefaultAuthenticationSchemeName)
            {
                if (user!.HasClaim(nameof(ApiKeyType), Enum.GetName(typeof(ApiKeyType), ApiKeyType.Admin)! ))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }
            if (scheme == CookieAuthenticationDefaults.AuthenticationScheme)
            {
                if (user!.FindFirst(ClaimTypes.NameIdentifier)?.Value == resource)
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }
            context.Fail();
            return Task.CompletedTask;
        }
    }
}
