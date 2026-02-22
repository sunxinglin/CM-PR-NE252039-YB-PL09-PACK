using Ctp0600P.Shared.NotificationDTO;
using MediatR;
using Yee.Common.Library.CommonEnum;

namespace Ctp0600P.Client.Notifications
{
    public class AgvMsgNotification : INotification
    {
        public AgvMsgNotification(AgvMsgContextNotification msg)
        {
            PackOutCode = msg.PackOutCode ?? "";
            StationCode = msg.StationCode;
            AgvNo = msg.AgvNo;
            Action = msg.Action;
        }
        public string StationCode { get; set; }
        public string PackOutCode { get; set; }
        public int AgvNo { get; set; }
        public AgvActionEnum Action { get; set; }
    }
}
