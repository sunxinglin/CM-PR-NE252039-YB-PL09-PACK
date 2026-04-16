using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

using AsZero.Core.Entities;
using AsZero.DbContexts;

using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AsZero.Services.Auth
{
    public class ApiKeyAuthencationHandler : AuthenticationHandler<ApiKeyAuthOpts>
    {
        private readonly AsZeroDbContext _dbContext;

        public ApiKeyAuthencationHandler(AsZeroDbContext dbContext, IOptionsMonitor<ApiKeyAuthOpts> optionsMonitor, ILoggerFactory loggerFactory, UrlEncoder encoder, ISystemClock clock)
            : base(optionsMonitor, loggerFactory, encoder, clock)
        {
            this._dbContext = dbContext;
        }

        protected virtual Task<(bool, ApiKey?)> ValidateApiKeyAsync(string credentials)
        {
            try
            {
                //Extract credentials
                string idKey = Encoding.UTF8.GetString(Convert.FromBase64String(credentials));
                int sepIdex = idKey.IndexOf(':');
                var id = idKey[..sepIdex];
                var key = idKey[(sepIdex + 1)..];
                // validate identifier with key
                var s = this._dbContext.ApiKeys
                    .AsNoTracking()
                    .FirstOrDefault(k => k.Token == key && k.ClientIdentifier == id && k.Valid);
                var res = s == null ? (false, null) : (true, s);
                return Task.FromResult(res);
            }
            catch
            {
                (bool, ApiKey?) res = (false, null);
                return Task.FromResult(res);
            }
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // 获取Key
            string? token = null;
            string authorization = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorization))
            {
                // 回退到URI
                if (!Request.Query.TryGetValue("auth", out var newauth))
                {
                    return AuthenticateResult.NoResult();
                }
                authorization = newauth.FirstOrDefault() ?? "";
            }
            if (authorization.StartsWith(this.Options.KeyPrefix, StringComparison.OrdinalIgnoreCase))
            {
                token = authorization[Options.KeyPrefix.Length..].Trim();
            }

            if (string.IsNullOrEmpty(token)) { return AuthenticateResult.NoResult(); }

            // 校验token
            var (flag, apiKey) = await this.ValidateApiKeyAsync(token);
            if (!flag)
            {
                return AuthenticateResult.Fail($"token {this.Options.KeyPrefix} not match");
            }
            else
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, apiKey!.ClientIdentifier),
                    new Claim(nameof(ApiKeyType), Enum.GetName(typeof(ApiKeyType), apiKey.ApiKeyType)!)
                };
                var id = new ClaimsIdentity(claims, Scheme.Name);
                ClaimsPrincipal principal = new(id);
                var ticket = new AuthenticationTicket(principal, new AuthenticationProperties(), Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
        }

    }
}
