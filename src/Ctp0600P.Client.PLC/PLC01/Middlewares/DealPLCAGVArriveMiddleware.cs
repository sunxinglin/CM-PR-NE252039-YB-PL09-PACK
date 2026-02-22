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
    public class DealPLCAGVArriveMiddleware : IWorkMiddleware<ScanContext>
    {
        private readonly IMediator _mediator;
        private readonly ITraceApi _traceApi;
        private readonly IConfiguration _configuration;

        public DealPLCAGVArriveMiddleware(ILogger<DealPLCAGVArriveMiddleware> logger, IMediator mediator, ITraceApi traceApi, IConfiguration configuration)
        {
            _mediator = mediator;
            _traceApi = traceApi;
            _configuration = configuration;
        }

        public async Task InvokeAsync(ScanContext context, WorkDelegate<ScanContext> next)
        {
            var req = context.DevMsg.AGVArrive.Flag.HasFlag(RequestFlag.Req);
            var ack = context.MstMsg.AGVArrive.Flag.HasFlag(ResponseFlag.Ack);

            //处理请求
            if (req && !ack)
            {
                var agvNo = context.DevMsg.AGVArrive.AGVNo;
                var packCode = context.DevMsg.AGVArrive.PackCode.EffectiveContent;
                var stationCode = _configuration.GetSection("StepSetting").GetValue<string>("StationCode");

                //发送日志消息
                await _mediator.Publish(new UILogNotification(new LogMessage { Content = $"[AGV到站]收到请求，车号:{agvNo}, 条码:{packCode}, 工位:{stationCode}", Level = LogLevel.Information, Timestamp = DateTime.Now }));

                //判断参数是否异常
                if (agvNo <= 0)
                {
                    await _mediator.Publish(new UILogNotification(new LogMessage { Content = $"[AGV到站]请求异常，车号:{agvNo}, 条码:{packCode}, 工位:{stationCode}", Level = LogLevel.Error, Timestamp = DateTime.Now }));
                }
                else
                {
                    //调用相关接口
                    var resp = await _traceApi.AGVArrived(new AGVMessage { AgvCode = agvNo, ProductCode = packCode, StationName = stationCode, });
                    if (resp.Code != 200)
                    {
                        await _mediator.Publish(new UILogNotification(new LogMessage { Content = $"[AGV到站]调用接口失败，{JsonSerializer.Serialize(resp)}", Level = LogLevel.Warning, Timestamp = DateTime.Now }));
                    }
                    else
                    {
                        //确认消息
                        context.MstMsg.AGVArrive.Flag = new ResponseFlagBuilder(context.MstMsg.AGVArrive.Flag).Ack(true).Build();
                        await _mediator.Publish(new UILogNotification(new LogMessage { Content = $"[AGV到站]完成请求，{JsonSerializer.Serialize(resp)}", Level = LogLevel.Warning, Timestamp = DateTime.Now }));
                    }
                }
            }

            //重置请求
            if (!req && ack)
            {
                context.MstMsg.AGVArrive.Flag = new ResponseFlagBuilder(context.MstMsg.AGVArrive.Flag)
                    .Reset()
                    .Build();
            }

            await next(context);
        }
    }
}
