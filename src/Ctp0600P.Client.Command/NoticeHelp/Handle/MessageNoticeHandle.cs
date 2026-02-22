
using MediatR;
using SDKSD.Client.Commom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Ctp0600P.Client.Command.NoticeHelp.Handle
{
    public class MessageNoticeHandle : INotificationHandler<MessageNotice>
    {
        public Task Handle(MessageNotice notification, CancellationToken cancellationToken)
        {
            switch (notification.showType)
            {
                case Enum.MessageShowType.顶部弹窗:
                    var element = System.Windows.Markup.XamlReader.Parse(UIHelp.GetNoticeXaml("提示：", notification.MessageStr)) as UIElement;
                    


                    break;
                case Enum.MessageShowType.右下角弹窗:
                    UIHelp.ShowNotice(notification.messageType, notification.MessageStr);
                    break;
                default:
                    break;
            }
            return Task.CompletedTask;
        }
    }
}
