using AsZero.Core.Services.Messages;

using MediatR;

namespace AutomaticStation.WebApi.Handlers
{
    public class UILogNotificationHandler : INotificationHandler<UILogNotification>
    {
        private readonly ILogger<UILogNotificationHandler> _logger;

        public UILogNotificationHandler(ILogger<UILogNotificationHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(UILogNotification notification, CancellationToken cancellationToken)
        {
            var msg = notification.LogMessage;
            //if (msg != null && msg.Level >= LogLevel.Information)
            //{
            //    this._vm.OnNext(msg);
            //}
            //_logger.LogInformation(msg.Content);

            _logger.Log(msg.Level, msg.Content);
            return Task.CompletedTask;
        }
    }
}
