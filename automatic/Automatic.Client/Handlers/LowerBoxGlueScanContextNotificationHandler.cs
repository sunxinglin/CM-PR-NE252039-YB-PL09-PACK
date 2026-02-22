using Automatic.Client.ViewModels.Realtime;
using Automatic.Protocols.LowerBoxGlue.Middlewares.Common.PublishNotification;
using Automatic.Protocols.LowerBoxGlue.Models.Wraps;
using Automatic.Protocols.LowerBoxGlue;
using MediatR;

namespace Automatic.Client.Handlers
{
    internal class LowerBoxGlueScanContextNotificationHandler : INotificationHandler<ScanContextNotification>, INotificationHandler<DealReqGlueCompleteWraps>
    {
        private readonly LowerBoxGlueMonitorViewModel _monitorViewModel;
        private readonly IServiceProvider _sp;

        public LowerBoxGlueScanContextNotificationHandler(LowerBoxGlueMonitorViewModel monitorViewModel, IServiceProvider sp)
        {
            _monitorViewModel = monitorViewModel;
            _sp = sp;
        }

        public Task Handle(ScanContextNotification notification, CancellationToken cancellationToken)
        {
            var ctx = new ScanContext(_sp, notification.DevMsg, notification.MstMsg, notification.CreatedAt);
            _monitorViewModel.ScanContextSubject.OnNext(ctx);
            return Task.CompletedTask;
        }

        public Task Handle(DealReqGlueCompleteWraps notification, CancellationToken cancellationToken)
        {
            _monitorViewModel.GlueDataSubject.OnNext(notification);
            return Task.CompletedTask;
        }
    }
}
