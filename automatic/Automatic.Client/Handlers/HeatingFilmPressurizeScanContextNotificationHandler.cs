using Automatic.Client.ViewModels.Realtime;
using Automatic.Protocols.HeatingFilmPressurize;
using Automatic.Protocols.HeatingFilmPressurize.Middlewares.Common.PublishNotification;
using Automatic.Protocols.HeatingFilmPressurize.Models.Wraps;
using MediatR;

namespace Automatic.Client.Handlers
{
    internal class HeatingFilmPressurizeScanContextNotificationHandler : INotificationHandler<ScanContextNotification>, INotificationHandler<DealReqPressurizeCompleteWraps>
    {
        private readonly HeatingFilmPressurizeMonitorViewModel _monitorViewModel;
        private readonly IServiceProvider _sp;

        public HeatingFilmPressurizeScanContextNotificationHandler(HeatingFilmPressurizeMonitorViewModel monitorViewModel, IServiceProvider sp)
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
