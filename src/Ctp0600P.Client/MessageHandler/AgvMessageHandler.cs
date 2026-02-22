using Ctp0600P.Client.Notifications;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Ctp0600P.Client.MessageHandler
{
    public class AgvMessageHandler : INotificationHandler<AgvMsgNotification>
    {
        public AgvMessageHandler()
        {

        }

        public Task Handle(AgvMsgNotification notification, CancellationToken cancellationToken)
        {
            App.CatchAgvMessage(notification);
            return Task.CompletedTask;
        }
    }
}
