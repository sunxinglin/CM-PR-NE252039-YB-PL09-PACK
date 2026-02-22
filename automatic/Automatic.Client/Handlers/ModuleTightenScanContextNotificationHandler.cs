using Automatic.Client.ViewModels.Realtime;
using Automatic.Protocols.ModuleTighten;
using Automatic.Protocols.ModuleTighten.Middlewares.Common.PublishNotification;
using Automatic.Protocols.ModuleTighten.Models.Wraps;
using MediatR;

namespace Automatic.Client.Handlers
{
    public class ModuleTightenScanContextNotificationHandler : INotificationHandler<ScanContextNotification>, INotificationHandler<DealReqTightenCompleteWraps>
    {
        private readonly ModuleTightenMonitorViewModel _monitorViewModel;
        private readonly IServiceProvider _sp;

        public ModuleTightenScanContextNotificationHandler(ModuleTightenMonitorViewModel monitorViewModel, IServiceProvider sp)
        {
            _monitorViewModel = monitorViewModel;
            _sp = sp;
        }
        public Task Handle(ScanContextNotification notification, CancellationToken cancellationToken)
        {
            var scanContext = new ScanContext(_sp, notification.DevMsg, notification.MstMsg, DateTimeOffset.Now);
            _monitorViewModel.ScanContextSubject.OnNext(scanContext);
            return Task.CompletedTask;
        }

        public Task Handle(DealReqTightenCompleteWraps notification, CancellationToken cancellationToken)
        {
            _monitorViewModel.TightenDataSubject.OnNext(notification);
            return Task.CompletedTask;
        }
    }
}
