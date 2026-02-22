using System.Security.Claims;

namespace AsZero.Core.Services.Auth
{
    public interface IPrincipalAccessor
    {
        ClaimsPrincipal GetCurrentPrincipal();
        void SetCurrentPrincipal(ClaimsPrincipal principal);
    }
}
