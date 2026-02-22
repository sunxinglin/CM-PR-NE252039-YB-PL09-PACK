using Ctp0600P.Client.ObservableMonitor;
using Ctp0600P.Client.PLC.PLC01;
using Ctp0600P.Client.PLC.PLC01.Middlewares.Common.PublishNotification;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Ctp0600P.Client.MessageHandler
{
    public class PLC01NotificationHandler : INotificationHandler<ScanContextNotification>
    {
        private readonly PLC01ObservableMonitor _ObservableMonitor;

        public PLC01NotificationHandler(PLC01ObservableMonitor observableMonitor)
        {
            _ObservableMonitor = observableMonitor;
        }

        public Task Handle(ScanContextNotification notification, CancellationToken cancellationToken)
        {
            _ObservableMonitor.OnNextContext(new ScanContext(null, notification.DevMsg, notification.MstMsg, notification.CreatedAt));
            _ObservableMonitor.OnNextHearted(notification.CreatedAt);

            return Task.CompletedTask;
        }
    }
}
