using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Linq;

namespace Yee.Services.Auth
{
    public class AgvAuthOpts : AuthenticationSchemeOptions
    {
        public const string DefaultAuthenticationSchemeName = "AGV_Auth_Scheme";
        public string KeyPrefix = "AGV";
        public string ByPassKey = string.Empty;
    }
}
