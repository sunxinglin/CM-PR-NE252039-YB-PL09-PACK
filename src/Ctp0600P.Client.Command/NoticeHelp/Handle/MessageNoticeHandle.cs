using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

using Ctp0600P.Client.Command.NoticeHelp.Enum;

using MediatR;

using SDKSD.Client.Commom;

namespace Ctp0600P.Client.Command.NoticeHelp.Handle;

public class MessageNoticeHandle : INotificationHandler<MessageNotice>
{
    public Task Handle(MessageNotice notification, CancellationToken cancellationToken)
    {
        switch (notification.showType)
        {
            case MessageShowType.顶部弹窗:
                var element = XamlReader.Parse(UIHelp.GetNoticeXaml("提示：", notification.MessageStr)) as UIElement;
                break;
            case MessageShowType.右下角弹窗:
                UIHelp.ShowNotice(notification.messageType, notification.MessageStr);
                break;
            default:
                break;
        }
        return Task.CompletedTask;
    }
}