using AsZero.Core.Services.Messages;
using Ctp0600P.Shared;
using Ctp0600P.Shared.NotificationDTO;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Yee.WebApi.Hubs;

namespace Yee.WebApi.MessageHandlers
{
    public class AlarmLogNotificationHandler : INotificationHandler<UILogNotification>
    {
        private readonly IHubContext<AllMessageHub, IAllMessageHubClient> _hubContext;
        public AlarmLogNotificationHandler(IHubContext<AllMessageHub, IAllMessageHubClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(UILogNotification notification, CancellationToken cancellationToken)
        {
            var msg = notification.LogMessage;

            if (msg is AlarmMessage alarmmsg)
            {
                await _hubContext.Clients.All.ReceiveUILogAlarmNotification(alarmmsg);
            }
            else if (msg is LogMessage logmsg)
            {
                await _hubContext.Clients.All.ReceiveUILogMessageNotification(logmsg);
            }
        }
    }
}
