using Automatic.Protocols.LowerBoxGlue.Models;

namespace Automatic.Protocols.Hubs.Models.NotificationDTO
{
    public class LowerBoxGlueNotificationDTO
    {
        public DevMsg DevMsg { get; set; }
        public MstMsg MstMsg { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
