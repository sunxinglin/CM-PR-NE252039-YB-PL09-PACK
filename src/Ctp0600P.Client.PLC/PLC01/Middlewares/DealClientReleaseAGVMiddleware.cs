using AsZero.Core.Services.Messages;
using Ctp0600P.Client.PLC.Common;
using Ctp0600P.Client.PLC.Context;
using Itminus.Middlewares;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ctp0600P.Client.PLC.PLC01.Middlewares
{
    public class DealClientReleaseAGVMiddleware : IWorkMiddleware<ScanContext>
    {
        private readonly StationPLCContext _stationPLCContext;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public DealClientReleaseAGVMiddleware(ILogger<DealClientReleaseAGVMiddleware> logger, IMediator mediator, StationPLCContext stationPLCContext, IConfiguration configuration)
        {
            _stationPLCContext = stationPLCContext;
            _mediator = mediator;
            _configuration = configuration;
        }

        public async Task InvokeAsync(ScanContext context, WorkDelegate<ScanContext> next)
        {
            var mstReq = context.MstMsg.ReleaseAGV.Flag.HasFlag(RequestFlag.Req);
            var devAck = context.DevMsg.ReleaseAGV.Flag.HasFlag(ResponseFlag.Ack);

            //程序发起请求，写入PLC
            var reqs = _stationPLCContext.ReleaseAGVReqs;
            for (int i = 0; i < reqs.Count; i++)
            {
                var req = reqs[i];

                //如果当前已有请求，则不再处理新的请求
                if (mstReq || devAck)
                {
                    break;
                }

                context.MstMsg.ReleaseAGV.Flag = new RequestFlagBuilder(context.MstMsg.ReleaseAGV.Flag).Req(true).Build();

                reqs.Remove(req);
                //发送日志消息
                await _mediator.Publish(new UILogNotification(new LogMessage { Content = $"[放行AGV]发起请求", Level = LogLevel.Information, Timestamp = DateTime.Now }));
            }

            //PLC应答了上位机发起的请求，关闭请求
            if (mstReq && devAck)
            {
                //更新Pending消息
                context.MstMsg.ReleaseAGV.Flag = new RequestFlagBuilder(context.MstMsg.ReleaseAGV.Flag).Req(false).Build();
                await _mediator.Publish(new UILogNotification(new LogMessage { Content = $"[放行AGV]完成请求", Level = LogLevel.Information, Timestamp = DateTime.Now }));
            }

            await next(context);
        }
    }
}
