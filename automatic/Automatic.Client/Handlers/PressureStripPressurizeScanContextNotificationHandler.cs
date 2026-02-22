using Automatic.Client.ViewModels.Realtime;
using Automatic.Protocols.PressureStripPressurize;
using Automatic.Protocols.PressureStripPressurize.Middlewares.Common.PublishNotification;
using Automatic.Protocols.PressureStripPressurize.Models.Wraps;
using MediatR;

namespace Automatic.Client.Handlers
{
    internal class PressureStripPressurizeScanContextNotificationHandler : INotificationHandler<ScanContextNotification>, INotificationHandler<DealReqPressurizeCompleteWraps>
    {
        private readonly PressureStripPressurizeMonitorViewModel _monitorViewModel;
        private readonly IServiceProvider _sp;

        public PressureStripPressurizeScanContextNotificationHandler(PressureStripPressurizeMonitorViewModel monitorViewModel, IServiceProvider sp)
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

        public Task Handle(DealReqPressurizeCompleteWraps notification, CancellationToken cancellationToken)
        {
            _monitorViewModel.ReshapeWrapSubject.OnNext(notification);
            return Task.CompletedTask;
        }
    }
}
