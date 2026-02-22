using Microsoft.AspNetCore.Authentication;

namespace AsZero.Services.Auth
{
    public class ApiKeyAuthOpts : AuthenticationSchemeOptions
    {
        public const string DefaultAuthenticationSchemeName = "Ctp0600PackApiKey.AuthScheme";
        public string KeyPrefix = "Ctp0600Pack.Key";
    }
}
