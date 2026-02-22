using AsZero.Core.Services.Messages;
using Automatic.Client.ViewModels.Realtime;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;

namespace Automatic.Client.Handlers
{
    internal class UILogNotificationHandler : INotificationHandler<UILogNotification>
    {
        private readonly UILogsViewModel _vm;
        private readonly ILogger<UILogNotificationHandler> _logger;

        public UILogNotificationHandler(UILogsViewModel vm, ILogger<UILogNotificationHandler> logger)
        {
            _vm = vm;
            _logger = logger;
        }

        public Task Handle(UILogNotification notification, CancellationToken cancellationToken)
        {
            var msg = notification.LogMessage;
            if (msg != null)
            {
                _vm.OnNext(msg);
            }
            _logger.Log(msg.Level, msg.Content);
            //Log.Logger = new LoggerConfiguration().WriteTo;

            return Task.CompletedTask;
        }
    }
}
