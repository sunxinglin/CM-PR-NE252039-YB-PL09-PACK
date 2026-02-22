using Ctp0600P.Client.IO;
using Ctp0600P.Client.PLC.PLC01.Models.Notifications;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Ctp0600P.Client.MessageHandler
{
    public class StationPLCStatusNotificationHandler : INotificationHandler<StationPLCStatusNotification>
    {
        private readonly IOBoxBusinessProcess _ioBoxBusinessProcess;

        public StationPLCStatusNotificationHandler(IOBoxBusinessProcess iOBoxBusinessProcess)
        {
            _ioBoxBusinessProcess = iOBoxBusinessProcess;
        }

        public async Task Handle(StationPLCStatusNotification notification, CancellationToken cancellationToken)
        {
            await _ioBoxBusinessProcess.Process(notification);
        }
    }
}
