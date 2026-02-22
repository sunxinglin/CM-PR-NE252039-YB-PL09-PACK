using AsZero.Core.Services.Messages;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Ctp0600P.Client.MessageHandler
{
    internal class MessageBoxNotificationHandler : INotificationHandler<MessageBoxNotification>
    {
        private readonly ILogger<MessageBoxNotificationHandler> _logger;

        public MessageBoxNotificationHandler(ILogger<MessageBoxNotificationHandler> logger)
        {
            this._logger = logger;
        }

        public Task Handle(MessageBoxNotification notification, CancellationToken cancellationToken)
        {

            MessageBox.Show(notification.Content);
            return Task.CompletedTask;
        }
    }
}
