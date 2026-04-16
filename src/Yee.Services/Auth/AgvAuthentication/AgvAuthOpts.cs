using Microsoft.AspNetCore.Authentication;

namespace Yee.Services.Auth
{
    public class AgvAuthOpts : AuthenticationSchemeOptions
    {
        public const string DefaultAuthenticationSchemeName = "AGV_Auth_Scheme";
        public string KeyPrefix = "AGV";
        public string ByPassKey = string.Empty;
    }
}
