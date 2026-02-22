using Automatic.Protocols.Hubs.Models.NotificationDTO;

namespace Automatic.Protocols.Hubs
{
    public interface IMessageHubClient
    {
        Task ReceiveLowerBoxGlue(LowerBoxGlueNotificationDTO dto);
    }
}
