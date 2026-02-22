using Automatic.Client.ViewModels.Realtime;
using Automatic.Protocols.UpperCoverTighten;
using Automatic.Protocols.UpperCoverTighten.Middlewares.Common.PublishNotification;
using Automatic.Protocols.UpperCoverTighten.Models.Wraps;
using MediatR;

namespace Automatic.Client.Handlers
{
    internal class UpperCoverTightenScanContextNotificationHandler : INotificationHandler<ScanContextNotification>, INotificationHandler<DealReqTightenCompleteWraps>
    {
        private readonly UpperCoverTighten1MonitorViewModel _monitorViewModel;
        private readonly IServiceProvider _sp;

        public UpperCoverTightenScanContextNotificationHandler(UpperCoverTighten1MonitorViewModel monitorViewModel, IServiceProvider sp)
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

        public Task Handle(DealReqTightenCompleteWraps notification, CancellationToken cancellationToken)
        {
            _monitorViewModel.TightenDataSubject.OnNext(notification);
            return Task.CompletedTask;
        }
    }
}
