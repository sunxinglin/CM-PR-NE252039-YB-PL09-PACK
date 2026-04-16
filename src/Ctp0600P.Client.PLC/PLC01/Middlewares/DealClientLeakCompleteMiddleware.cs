using System.Text.Json;

using AsZero.Core.Services.Messages;

using Ctp0600P.Client.PLC.Common;
using Ctp0600P.Client.PLC.PLC01.Models.Notifications;

using Itminus.Middlewares;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Ctp0600P.Client.PLC.PLC01.Middlewares
{
    public class DealClientLeakCompleteMiddleware : IWorkMiddleware<ScanContext>
    {
        private readonly IMediator _mediator;

        public DealClientLeakCompleteMiddleware(ILogger<DealClientLeakCompleteMiddleware> logger, IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task InvokeAsync(ScanContext context, WorkDelegate<ScanContext> next)
        {
            var LeakCompleteReq = context.DevMsg.LeakComplete.Flag.HasFlag(RequestFlag.Req);
            var LeakCompleteAck = context.MstMsg.LeakComplete.Flag.HasFlag(ResponseFlag.Ack);

            //上位机发起拧紧请求，PLC应答
            if (LeakCompleteReq && !LeakCompleteAck)
            {
                //把完成的消息发送出去
                var notification = new StationPLCLeakResultNotification()
                {
                    LeakStartTime = context.DevMsg.LeakComplete.LeakStartTime,
                    LeakCompleteTime = context.DevMsg.LeakComplete.LeakCompleteTime,
                    LeakTotalPower = context.DevMsg.LeakComplete.LeakTotalPower,
                    LeakProportionalPressure = context.DevMsg.LeakComplete.LeakProportionalPressure,
                    LeakAdkerPressure = context.DevMsg.LeakComplete.LeakAdkerPressure,
                    LeakTime = context.DevMsg.LeakComplete.LeakTime,
                    LeakKeepTime = context.DevMsg.LeakComplete.LeakKeepTime,
                    LeakPressPower = context.DevMsg.LeakComplete.LeakPressPower,
                    LeakKeepPressPower = context.DevMsg.LeakComplete.LeakKeepPressPower,
                    LeakRealTime = context.DevMsg.LeakComplete.LeakRealTime
                };
                await _mediator.Publish(notification);


                //更新Pending消息
                context.MstMsg.LeakComplete.Flag = new ResponseFlagBuilder(context.MstMsg.LeakComplete.Flag)
                    .Ack(true)
                    .Build();

                //发送日志
                await _mediator.Publish(new UILogNotification(new LogMessage
                {
                    Content = $"[充气完成]收到请求，{JsonSerializer.Serialize(notification)}",
                    Level = LogLevel.Information,
                    Timestamp = DateTime.Now
                }));
            }

            //重置请求
            if (!LeakCompleteReq && LeakCompleteAck)
            {
                context.MstMsg.LeakComplete.Flag = new ResponseFlagBuilder(context.MstMsg.LeakComplete.Flag)
                    .Reset()
                    .Build();
            }

            await next(context);
        }
    }
}
