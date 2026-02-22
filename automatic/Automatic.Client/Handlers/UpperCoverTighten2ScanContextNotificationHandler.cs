using Automatic.Client.ViewModels.Realtime;
using Automatic.Protocols.UpperCoverTighten2;
using Automatic.Protocols.UpperCoverTighten2.Middlewares.Common.PublishNotification;
using Automatic.Protocols.UpperCoverTighten2.Models.Wraps;
using MediatR;

namespace Automatic.Client.Handlers
{
    internal class UpperCoverTighten2ScanContextNotificationHandler : INotificationHandler<ScanContextNotification>, INotificationHandler<DealReqTightenCompleteWraps>
    {
        private readonly UpperCoverTighten2MonitorViewModel _monitorViewModel;
        private readonly IServiceProvider _sp;

        public UpperCoverTighten2ScanContextNotificationHandler(UpperCoverTighten2MonitorViewModel monitorViewModel, IServiceProvider sp)
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
