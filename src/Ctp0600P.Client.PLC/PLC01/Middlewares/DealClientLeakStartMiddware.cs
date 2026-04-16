using System.Text.Json;

using AsZero.Core.Services.Messages;

using Ctp0600P.Client.PLC.Common;
using Ctp0600P.Client.PLC.Context;

using Itminus.Middlewares;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Ctp0600P.Client.PLC.PLC01.Middlewares
{
    public class DealClientLeakStartMiddware : IWorkMiddleware<ScanContext>
    {
        private readonly StationPLCContext _manualStationNotifyPLCContext;
        private readonly IMediator _mediator;

        public DealClientLeakStartMiddware(ILogger<DealClientLeakStartMiddware> logger, IMediator mediator,
            StationPLCContext manualStationNotifyPLCContext)
        {
            _manualStationNotifyPLCContext = manualStationNotifyPLCContext;
            _mediator = mediator;
        }

        public async Task InvokeAsync(ScanContext context, WorkDelegate<ScanContext> next)
        {
            var mstReq = context.MstMsg.LeakStart.Flag.HasFlag(RequestFlag.Req);

            var devAck = context.DevMsg.LeakStart.Flag.HasFlag(ResponseFlag.Ack);
            var devOK = context.DevMsg.LeakStart.Flag.HasFlag(ResponseFlag.OK);
            var devNG = context.DevMsg.LeakStart.Flag.HasFlag(ResponseFlag.NG);

            //程序发起充气请求，写入PLC
            var LeakReq = _manualStationNotifyPLCContext.StationLeakStartReqs;
            if (LeakReq.Req)
            {
                context.MstMsg.LeakStart.LeakDuration = LeakReq.LeakDuration;
                context.MstMsg.LeakStart.PressureDuration = LeakReq.PressureDuration;
                context.MstMsg.LeakStart.LeakStress = LeakReq.LeakStress;
                context.MstMsg.LeakStart.PressureStress = LeakReq.PressureStress;
                context.MstMsg.LeakStart.Flag = new RequestFlagBuilder(context.MstMsg.LeakStart.Flag).Req(true).Build();

                //发送日志消息
                await _mediator.Publish(new UILogNotification(new LogMessage
                {
                    Content = $"[充气开始]充气请求，{JsonSerializer.Serialize(LeakReq)}",
                    Level = LogLevel.Debug,
                    Timestamp = DateTime.Now
                }));

                //初始化客户端上下文
                LeakReq.Req = false;
                LeakReq.LeakDuration = 0;
                LeakReq.PressureDuration = 0;
                LeakReq.LeakStress = 0;
                LeakReq.PressureStress = 0;
            }

            //PLC应答了上位机发起拧紧请求，关闭请求
            if (mstReq && devAck)
            {
                //更新Pending消息
                context.MstMsg.LeakStart.LeakDuration = 0;
                context.MstMsg.LeakStart.PressureDuration = 0;
                context.MstMsg.LeakStart.LeakStress = 0;
                context.MstMsg.LeakStart.PressureStress = 0;
                context.MstMsg.LeakStart.Flag = new RequestFlagBuilder(context.MstMsg.LeakStart.Flag).Req(false).Build();

                //发送日志消息
                await _mediator.Publish(new UILogNotification(new LogMessage
                {
                    Content = $"[充气开始]完成请求",
                    Level = LogLevel.Information,
                    Timestamp = DateTime.Now
                }));

            }

            await next(context);
        }
    }
}
