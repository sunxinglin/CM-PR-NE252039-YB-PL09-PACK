using System.Threading;
using System.Threading.Tasks;

using Ctp0600P.Client.Notifications;

using MediatR;

namespace Ctp0600P.Client.MessageHandler;

public class AgvMessageHandler : INotificationHandler<AgvMsgNotification>
{
    public AgvMessageHandler()
    {

    }

    public Task Handle(AgvMsgNotification notification, CancellationToken cancellationToken)
    {
        // 收到内部广播后，转交给 App 类的静态方法处理
        App.CatchAgvMessage(notification);
        return Task.CompletedTask;
    }
}