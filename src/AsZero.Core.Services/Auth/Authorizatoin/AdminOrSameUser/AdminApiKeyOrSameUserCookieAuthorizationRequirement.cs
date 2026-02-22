using Microsoft.AspNetCore.Authorization;

namespace AsZero.Services.Auth
{
    public class AdminApiKeyOrSameUserCookieAuthorizationRequirement : IAuthorizationRequirement
    {
    }
}
