using System.Threading;
using System.Threading.Tasks;

using Ctp0600P.Client.ObservableMonitor;
using Ctp0600P.Client.PLC.PLC01;
using Ctp0600P.Client.PLC.PLC01.Middlewares.Common.PublishNotification;

using MediatR;

namespace Ctp0600P.Client.MessageHandler;

public class PLC01NotificationHandler : INotificationHandler<ScanContextNotification>
{
    private readonly PLC01ObservableMonitor _observableMonitor;

    public PLC01NotificationHandler(PLC01ObservableMonitor observableMonitor)
    {
        _observableMonitor = observableMonitor;
    }

    public Task Handle(ScanContextNotification notification, CancellationToken cancellationToken)
    {
        // notification是数据快照，因此要将其转换成系统业务逻辑中真正使用的上下文对象ScanContext
        _observableMonitor.OnNextContext(
            new ScanContext(null, notification.DevMsg, notification.MstMsg, notification.CreatedAt));
        _observableMonitor.OnNextHearted(notification.CreatedAt);

        return Task.CompletedTask;
    }
}