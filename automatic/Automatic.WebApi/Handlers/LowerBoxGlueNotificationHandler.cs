using AutomaticStation.Protocols.Hubs;
using AutomaticStation.Protocols.Hubs.Models.NotificationDTO;
using AutomaticStation.Protocols.LowerBoxGlue.Middlewares.Common.PublishNotification;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace AutomaticStation.WebApi.Handlers
{
    public class LowerBoxGlueNotificationHandler : INotificationHandler<ScanContextNotification>
    {
        private readonly IHubContext<MessageHub, IMessageHubClient> _hubContext;

        public LowerBoxGlueNotificationHandler(IHubContext<MessageHub, IMessageHubClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(ScanContextNotification notification, CancellationToken cancellationToken)
        {
            var dto = new LowerBoxGlueNotificationDTO
            {
                DevMsg = notification.DevMsg,
                MstMsg = notification.MstMsg,
                CreatedAt = notification.CreatedAt,
            };
            await _hubContext.Clients.All.ReceiveLowerBoxGlue(dto);
        }
    }
}
