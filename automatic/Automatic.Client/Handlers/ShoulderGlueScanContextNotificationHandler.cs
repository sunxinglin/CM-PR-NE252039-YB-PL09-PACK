using Automatic.Client.ViewModels.Realtime;
using Automatic.Protocols.ShoulderGlue;
using Automatic.Protocols.ShoulderGlue.Middlewares.Common.PublishNotification;
using Automatic.Protocols.ShoulderGlue.Models.Wraps;
using MediatR;

namespace Automatic.Client.Handlers
{
    internal class ShoulderGlueScanContextNotificationHandler : INotificationHandler<ScanContextNotification>, INotificationHandler<DealReqGlueCompleteWraps>
    {
        private readonly ShoulderGlueMonitorViewModel _monitorViewModel;
        private readonly IServiceProvider _sp;

        public ShoulderGlueScanContextNotificationHandler(ShoulderGlueMonitorViewModel monitorViewModel, IServiceProvider sp)
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
