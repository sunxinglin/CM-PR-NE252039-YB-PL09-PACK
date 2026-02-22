using Ctp0600P.Shared;
using Microsoft.AspNetCore.SignalR;

namespace Yee.WebApi.Hubs
{
    public class AllMessageHub : Hub<IAllMessageHubClient>
    {
        private readonly ILogger<AllMessageHub> _logger;

        public AllMessageHub(ILogger<AllMessageHub> logger)
        {
            this._logger = logger;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            this._logger.LogWarning($"客户端连接断开");
            return base.OnDisconnectedAsync(exception);
        }

    }

}
