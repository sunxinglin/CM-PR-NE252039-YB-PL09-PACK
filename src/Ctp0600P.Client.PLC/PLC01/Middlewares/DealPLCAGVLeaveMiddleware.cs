using AsZero.Core.Services.Messages;
using Ctp0600P.Client.PLC.Common;
using Ctp0600P.Client.Services;
using Itminus.Middlewares;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Yee.Entitys.DTOS;

namespace Ctp0600P.Client.PLC.PLC01.Middlewares
{
    public class DealPLCAGVLeaveMiddleware : IWorkMiddleware<ScanContext>
    {
        private readonly IMediator _mediator;
        private readonly ITraceApi _traceApi;
        private readonly IConfiguration _configuration;

        public DealPLCAGVLeaveMiddleware(ILogger<DealPLCAGVLeaveMiddleware> logger, IMediator mediator, ITraceApi traceApi, IConfiguration configuration)
        {
            _mediator = mediator;
            _traceApi = traceApi;
            _configuration = configuration;
        }

        public async Task InvokeAsync(ScanContext context, WorkDelegate<ScanContext> next)
        {
            var req = context.DevMsg.AGVLeave.Flag.HasFlag(RequestFlag.Req);
            var ack = context.MstMsg.AGVLeave.Flag.HasFlag(ResponseFlag.Ack);

            //处理请求
            if (req && !ack)
            {
                var agvNo = context.DevMsg.AGVLeave.AGVNo;
                var stationCode = _configuration.GetSection("StepSetting").GetValue<string>("StationCode");

                //发送日志消息
                await _mediator.Publish(new UILogNotification(new LogMessage { Content = $"[AGV离站]收到请求，车号:{agvNo}, 工位:{stationCode}", Level = LogLevel.Information, Timestamp = DateTime.Now }));

                //调用相关接口
                var resp = await _traceApi.AGVLeaved(new AGVMessage { AgvCode = agvNo, StationName = stationCode });
                if (resp.Code != 200)
                {
                    await _mediator.Publish(new UILogNotification(new LogMessage { Content = $"[AGV离站]调用接口失败，{JsonSerializer.Serialize(resp)}", Level = LogLevel.Warning, Timestamp = DateTime.Now }));
                }
                else
                {
                    //确认消息
                    context.MstMsg.AGVLeave.Flag = new ResponseFlagBuilder(context.MstMsg.AGVLeave.Flag).Ack(true).Build();
                    await _mediator.Publish(new UILogNotification(new LogMessage { Content = $"[AGV离站]完成请求，车号:{agvNo}, 工位:{stationCode}", Level = LogLevel.Information, Timestamp = DateTime.Now }));
                }
            }

            //重置请求
            if (!req && ack)
            {
                context.MstMsg.AGVLeave.Flag = new ResponseFlagBuilder(context.MstMsg.AGVLeave.Flag)
                    .Reset()
                    .Build();
            }

            await next(context);
        }
    }
}
