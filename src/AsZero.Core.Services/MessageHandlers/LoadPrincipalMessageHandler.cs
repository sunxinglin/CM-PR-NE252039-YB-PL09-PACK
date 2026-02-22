using AsZero.Core.Services.Auth;
using AsZero.Core.Services.Messages;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace AsZero.Core.Services.MessageHandlers
{
    public class LoadPrincipalMessageHandler : IRequestHandler<LoadPrincipalRequest, ClaimsPrincipal?>
    {
        private readonly IUserManager _userMgr;

        public LoadPrincipalMessageHandler(IUserManager userMgr)
        {
            this._userMgr = userMgr;
        }

        public async Task<ClaimsPrincipal?> Handle(LoadPrincipalRequest request, CancellationToken cancellationToken)
        {
            var principal = await _userMgr.LoadPrincipalAsync(request.Account, true);
            return principal;
        }
    }
}
