using Ctp0600P.Shared;
using Microsoft.AspNetCore.SignalR;

namespace Yee.WebApi.Hubs
{
    public class PlcHub : Hub<IAllMessageHubClient>
    {
        private readonly ILogger<PlcHub> _logger;

        public PlcHub(ILogger<PlcHub> logger)
        {
            this._logger = logger;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            this._logger.LogWarning($"PLC连接断开");
            return base.OnDisconnectedAsync(exception);
        }

    }

}
