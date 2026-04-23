using Ctp0600P.Client.PLC.PLC01.Models.Notifications;
using Ctp0600P.Client.Protocols.Leak.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ctp0600P.Client.MessageHandler
{
    /// <summary>
    /// PLC充气完成通知处理器
    /// </summary>
    public class StationPLCLeakCompleteNotificationHandler : INotificationHandler<StationPLCLeakResultNotification>
    {
        public async Task Handle(StationPLCLeakResultNotification notification, CancellationToken cancellationToken)
        {
            // 转换为 LeakCompleteRequest 发送到 Subject
            var request = new LeakCompleteRequest
            {
                LeakStartTime = notification.LeakStartTime,
                LeakCompleteTime = notification.LeakCompleteTime,
                LeakTotalPower = notification.LeakTotalPower,
                LeakProportionalPressure = notification.LeakProportionalPressure,
                LeakAdkerPressure = notification.LeakAdkerPressure,
                LeakTime = notification.LeakTime,
                LeakKeepTime = notification.LeakKeepTime,
                LeakPressPower = notification.LeakPressPower,
                LeakKeepPressPower = notification.LeakKeepPressPower,
            };

            // 发送到 Subject，触发 UI 处理
            App.LeakCompleteRequestSubject?.OnNext(request);

            await Task.CompletedTask;
        }
    }
}