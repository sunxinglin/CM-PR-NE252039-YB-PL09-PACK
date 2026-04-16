using MediatR;

using Yee.Common.Library.CommonEnum;

namespace Ctp0600P.Shared.NotificationDTO;

public class AgvMsgContextNotification : INotification
{
    public string PackOutCode { get; set; }
    public string StationCode { get; set; }
    public int AgvNo { get; set; }
    public AgvActionEnum Action { get; set; }
}