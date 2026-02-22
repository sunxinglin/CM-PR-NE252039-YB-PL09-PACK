using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Automatic.Protocols.Hubs
{
    public class MessageHub : Hub<IMessageHubClient>
    {
        private readonly ILogger<MessageHub> _logger;

        public MessageHub(ILogger<MessageHub> logger)
        {
            _logger = logger;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogWarning($"客户端连接断开");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
