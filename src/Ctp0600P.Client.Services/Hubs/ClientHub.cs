using Ctp0600P.Client.Protocols;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Yee.Entitys.AlarmMgmt;

namespace Yee.Services.Hubs
{
    //[Authorize(Policy = AuthDefines.Policy_SubstationApiKeyAuth)]
    public class ClientHub : Hub<IClientHub>
    {
        private readonly ILogger<ClientHub> _logger;
        private readonly IMediator mediator;

        public ClientHub(ILogger<ClientHub> logger,IMediator mediator)
        {
            this._logger = logger;
            this.mediator = mediator;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"客户端连接断开" });
            
            return base.OnDisconnectedAsync(exception);
        }
    }

}
