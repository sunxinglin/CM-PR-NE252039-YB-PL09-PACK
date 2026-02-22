using AsZero.Core.Services.Messages;
using Ctp0600P.Client.PLC.Common;
using Ctp0600P.Client.PLC.Context;
using Ctp0600P.Client.Services;
using Itminus.Middlewares;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Ctp0600P.Client.PLC.PLC01.Middlewares
{
    public class DealClientAGVBindPackMiddleware : IWorkMiddleware<ScanContext>
    {
        private readonly StationPLCContext _manualStationNotifyPLCContext;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly ITraceApi _traceApi;

        public DealClientAGVBindPackMiddleware(ILogger<DealClientAGVBindPackMiddleware> logger, IMediator mediator, StationPLCContext manualStationNotifyPLCContext, IConfiguration configuration, ITraceApi traceApi)
        {
            _manualStationNotifyPLCContext = manualStationNotifyPLCContext;
            _mediator = mediator;
            _configuration = configuration;
            _traceApi = traceApi;
        }

        public async Task InvokeAsync(ScanContext context, WorkDelegate<ScanContext> next)
        {
            var mstReq = context.MstMsg.AGVBindPack.Flag.HasFlag(RequestFlag.Req);
            var devAck = context.DevMsg.AGVBindPack.Flag.HasFlag(ResponseFlag.Ack);

            //如果当前已有请求，则不再处理新的请求
            if (mstReq && !devAck)
            {
                await next(context);
                return;
            }

            //程序发起AGV绑定请求，写入PLC
            var req = _manualStationNotifyPLCContext.AGVBindPackReq;
            if (req.Req)
            {
                context.MstMsg.AGVBindPack.Behavior = req.Behavior;
                context.MstMsg.AGVBindPack.AGVNo = req.AGVNo;
                context.MstMsg.AGVBindPack.PackCode = String40.New(req.PackCode);
                context.MstMsg.AGVBindPack.HolderBarcode = String40.New(req.HolderBarcode);
                context.MstMsg.AGVBindPack.StationCode = String40.New(req.StationCode);
                context.MstMsg.AGVBindPack.Flag = new RequestFlagBuilder(context.MstMsg.AGVBindPack.Flag).Req(true).Build();

                //发送日志消息
                await _mediator.Publish(new UILogNotification(new LogMessage { Content = $"[AGV绑定]发起请求，{JsonSerializer.Serialize(req)}", Level = LogLevel.Information, Timestamp = DateTime.Now }));

                //初始化客户端上下文
                req.Req = false;
                req.Behavior = default;
                req.AGVNo = default;
                req.PackCode = string.Empty;
                req.HolderBarcode = string.Empty;
                req.StationCode = string.Empty;
            }

            //PLC应答了上位机发起绑定请求，关闭请求
            if (mstReq && devAck)
            {
                //更新绑定数据
                var apiResult = await _traceApi.BingAgv(new Yee.Entitys.DTOS.BingAgvDTO
                {
                    State = context.MstMsg.AGVBindPack.Behavior == 1 ? Yee.Entitys.DTOS.BingAgvStateEnum.绑定 : Yee.Entitys.DTOS.BingAgvStateEnum.解绑,
                    AgvCode = context.MstMsg.AGVBindPack.AGVNo,
                    PackPN = context.MstMsg.AGVBindPack.PackCode.EffectiveContent,
                    HolderBarCode = context.MstMsg.AGVBindPack.HolderBarcode.EffectiveContent,
                    StationCode = context.MstMsg.AGVBindPack.StationCode.EffectiveContent,
                });

                var response = apiResult.Content;
                //如果成功，更新点位
                if (response?.Code == 200)
                {
                    await _mediator.Publish(new UILogNotification(new LogMessage { Content = $"[AGV绑定]完成请求", Level = LogLevel.Information, Timestamp = DateTime.Now }));

                    //更新Pending消息
                    context.MstMsg.AGVBindPack.Behavior = default;
                    context.MstMsg.AGVBindPack.AGVNo = default;
                    context.MstMsg.AGVBindPack.PackCode = String40.New(string.Empty);
                    context.MstMsg.AGVBindPack.HolderBarcode = String40.New(string.Empty);
                    context.MstMsg.AGVBindPack.StationCode = String40.New(string.Empty);
                    context.MstMsg.AGVBindPack.Flag = new RequestFlagBuilder(context.MstMsg.AGVBindPack.Flag).Req(false).Build();
                }
                else
                {
                    //发送日志消息
                    await _mediator.Publish(new UILogNotification(new LogMessage { Content = $"[AGV绑定]{response?.Message}", Level = LogLevel.Error, Timestamp = DateTime.Now }));
                }
            }

            await next(context);
        }
    }
}
