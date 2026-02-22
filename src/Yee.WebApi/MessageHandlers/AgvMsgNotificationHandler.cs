using Ctp0600P.Shared;
using Ctp0600P.Shared.NotificationDTO;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Yee.WebApi.Hubs;

namespace Yee.WebApi.MessageHandlers
{
    public class AgvMsgNotificationHandler : INotificationHandler<AgvMsgContextNotification>
    {
        private readonly IHubContext<AllMessageHub, IAllMessageHubClient> _hubContext;
        public AgvMsgNotificationHandler(IHubContext<AllMessageHub, IAllMessageHubClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(AgvMsgContextNotification notification, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.All.AgvActionMsg(notification);
        }
    }
}
