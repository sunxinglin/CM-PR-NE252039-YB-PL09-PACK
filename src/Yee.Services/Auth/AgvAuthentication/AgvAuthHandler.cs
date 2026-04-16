using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Yee.Services.Auth
{
    public class AgvAuthHandler : AuthenticationHandler<AgvAuthOpts>
    {
        public AgvAuthHandler(IOptionsMonitor<AgvAuthOpts> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string? token = null;
            string authorization = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorization))
            {
                return AuthenticateResult.NoResult();
            }
            if (authorization.StartsWith(this.Options.KeyPrefix, StringComparison.OrdinalIgnoreCase))
            {
                token = authorization.Substring(this.Options.KeyPrefix.Length).Trim();
            }

            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.Fail("token cannot be null");
            }
            var flag = await this.ValidateAgvTokenAsync(token);
            if (!flag)
            {
                return AuthenticateResult.Fail($"token {this.Options.KeyPrefix} not match");
            }
            else
            {
                var id = new ClaimsIdentity(
                    new Claim[] {
                        new Claim(ClaimTypes.NameIdentifier, "AGV"),
                    },
                    Scheme.Name
                );
                ClaimsPrincipal principal = new(id);
                var ticket = new AuthenticationTicket(principal, new AuthenticationProperties(), Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual Task<bool> ValidateAgvTokenAsync(string token)
        {
            if (token == "mentechs+AGV.key")
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
