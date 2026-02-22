using AsZero.Core.Services.Auth;
using AsZero.Core.Services.Messages;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsZero.Core.Services.MessageHandlers
{


    public class LoginMessageHandler : IRequestHandler<LoginRequest, LoginResponse>
    {
        private readonly IUserManager _userMgr;
        private readonly ILogger<LoginMessageHandler> _logger;
        private readonly IMediator _mediator;

        public LoginMessageHandler(IUserManager userMgr, ILogger<LoginMessageHandler> logger, IMediator mediator)
        {
            this._userMgr = userMgr;
            this._logger = logger;
            this._mediator = mediator;
        }

        public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            try { 
                var res = await _userMgr.ValidateUserAsync(request.Account, request.Password);

                LoginResponse resp;
                if (res?.Success == true)
                {
                    var principal = await this._mediator.Send( new LoadPrincipalRequest { Account = request.Account }, cancellationToken) ;
                    resp = new LoginResponse
                    {
                        Status = true,
                        Principal = principal,
                        Tip = res.Message,
                    };
                }
                else
                {
                    resp = new LoginResponse
                    {
                        Status = false,
                        Principal = null,
                        Tip = res!.Message,
                    };
                }
                return resp;
            }
            catch (Exception ex){
                this._logger.LogError($"处理用户登录消息出错: {ex.Message}\r\n{ex.StackTrace}");
                return new LoginResponse {
                    Status = false,
                    Principal = null,
                    Tip = ex.Message,
                };
            }
        }
    }
}
