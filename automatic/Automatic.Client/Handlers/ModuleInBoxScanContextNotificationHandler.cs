using Automatic.Client.ViewModels.Realtime;
using Automatic.Protocols.ModuleInBox.Middlewares.Common.PublishNotification;
using Automatic.Protocols.ModuleInBox.Models.Notifications;
using Automatic.Protocols.ModuleInBox;
using MediatR;

namespace Automatic.Client.Handlers
{
    internal class ModuleInBoxScanContextNotificationHandler : INotificationHandler<ScanContextNotification>, INotificationHandler<ModuleInBoxNotification>
    {
        private readonly ModuleInBox1MonitorViewModel _monitorViewModel;
        private readonly IServiceProvider _sp;

        public ModuleInBoxScanContextNotificationHandler(ModuleInBox1MonitorViewModel monitorViewModel, IServiceProvider sp)
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

        public Task Handle(ModuleInBoxNotification notification, CancellationToken cancellationToken)
        {
            _monitorViewModel.ModuleInBoxDataSubject.OnNext(notification.ModuleInBoxDatas);
            return Task.CompletedTask;
        }
    }
}
