using AsZero.Core.Services.Auth;
using AsZero.Core.Services.Messages;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AsZero.Core.Services.MessageHandlers
{


    public class LoginWithCardMessageHandler : IRequestHandler<LoginWithCardRequest, LoginResponse>
    {
        private readonly IUserManager _userMgr;
        private readonly ILogger<LoginMessageHandler> _logger;
        private readonly IMediator _mediator;

        public LoginWithCardMessageHandler(IUserManager userMgr, ILogger<LoginMessageHandler> logger, IMediator mediator)
        {
            this._userMgr = userMgr;
            this._logger = logger;
            this._mediator = mediator;
        }

        public async Task<LoginResponse> Handle(LoginWithCardRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var res = await _userMgr.GetUserAsync(request.Account);

                LoginResponse resp;
                if (res != null)
                {
                    var principal = await this._mediator.Send(new LoadPrincipalRequest { Account = request.Account }, cancellationToken);
                    resp = new LoginResponse
                    {
                        Status = true,
                        Principal = principal,
                        Tip = "登录成功",
                    };
                }
                else
                {
                    resp = new LoginResponse
                    {
                        Status = false,
                        Principal = null,
                        Tip = "不存在的员工卡",
                    };
                }
                return resp;
            }
            catch (Exception ex)
            {
                this._logger.LogError($"处理用户登录消息出错: {ex.Message}\r\n{ex.StackTrace}");
                return new LoginResponse
                {
                    Status = false,
                    Principal = null,
                    Tip = ex.Message,
                };
            }
        }
    }


    public class LoginWithCardRequest : IRequest<LoginResponse>, IBaseRequest
    {
        public string Account
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }
    }
}
