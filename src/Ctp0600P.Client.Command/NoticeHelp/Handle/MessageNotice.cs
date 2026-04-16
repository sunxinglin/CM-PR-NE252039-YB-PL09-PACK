using Ctp0600P.Client.Command.NoticeHelp.Enum;

using MediatR;

namespace Ctp0600P.Client.Command.NoticeHelp.Handle;

public class MessageNotice : INotification
{
    public MessageShowType showType { get; set; }
    public MessageTypeEnum messageType { get; set; }
    public string MessageStr { get; set; }
}