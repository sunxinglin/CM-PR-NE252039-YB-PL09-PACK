using Automatic.Client.ViewModels.Realtime;
using Automatic.Protocols.TerminalReshape;
using Automatic.Protocols.TerminalReshape.Middlewares.Common.PublishNotification;
using Automatic.Protocols.TerminalReshape.Models.Wraps;
using MediatR;

namespace Automatic.Client.Handlers
{
    public class TerminalReshapeScanContextNotification : INotificationHandler<ScanContextNotification>, INotificationHandler<DealReqCompleteReshapeWraps>
    {
        private readonly TerminalReshapeMonitorViewModel terminalReshapeMonitorViewModel;
        private readonly IServiceProvider _sp;
        public TerminalReshapeScanContextNotification(TerminalReshapeMonitorViewModel terminalReshapeMonitorViewModel, IServiceProvider sp)
        {
            this.terminalReshapeMonitorViewModel = terminalReshapeMonitorViewModel;
            _sp = sp;
        }

        public Task Handle(ScanContextNotification notification, CancellationToken cancellationToken)
        {
            var context = new ScanContext(_sp, notification.DevMsg, notification.MstMsg, DateTime.UtcNow);
            terminalReshapeMonitorViewModel.ScanContextSubject.OnNext(context);
            return Task.CompletedTask;
        }

        public Task Handle(DealReqCompleteReshapeWraps notification, CancellationToken cancellationToken)
        {
            terminalReshapeMonitorViewModel.ReshapeWrapSubject.OnNext(notification);
            return Task.CompletedTask;
        }
    }
}
