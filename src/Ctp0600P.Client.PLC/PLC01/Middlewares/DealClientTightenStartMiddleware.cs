using AsZero.Core.Services.Messages;
using Ctp0600P.Client.PLC.Common;
using Ctp0600P.Client.PLC.Context;
using Ctp0600P.Client.Protocols;
using Itminus.Middlewares;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Yee.Entitys.AlarmMgmt;

namespace Ctp0600P.Client.PLC.PLC01.Middlewares
{
    public class DealClientTightenStartMiddleware : IWorkMiddleware<ScanContext>
    {
        private readonly StationPLCContext _manualStationNotifyPLCContext;
        private readonly IMediator _mediator;

        public DealClientTightenStartMiddleware(ILogger<DealClientTightenStartMiddleware> logger, IMediator mediator, StationPLCContext manualStationNotifyPLCContext)
        {
            _manualStationNotifyPLCContext = manualStationNotifyPLCContext;
            _mediator = mediator;
        }

        public async Task InvokeAsync(ScanContext context, WorkDelegate<ScanContext> next)
        {
            var mstReq = context.MstMsg.TightenStart.Flag.HasFlag(RequestFlag.Req);

            var devAck = context.DevMsg.TightenStart.Flag.HasFlag(ResponseFlag.Ack);
            var devOK = context.DevMsg.TightenStart.Flag.HasFlag(ResponseFlag.OK);
            var devNG = context.DevMsg.TightenStart.Flag.HasFlag(ResponseFlag.NG);

            //程序发起拧紧请求，写入PLC
            var tightenReqs = _manualStationNotifyPLCContext.StationTightenStartReqs;

            for (int i = 0; i < tightenReqs.Count; i++)
            {
                var tightenReq = tightenReqs[i];

                //如果当前已有请求，则不再处理新的请求
                if (mstReq || devAck)
                {
                    break;
                }

                //发送日志消息
                await _mediator.Publish(new UILogNotification(new LogMessage
                {
                    Content = $"[拧紧开始]打印请求，{JsonSerializer.Serialize(tightenReqs)}",
                    Level = LogLevel.Debug,
                    Timestamp = DateTime.Now
                }));

                context.MstMsg.TightenStart.DeviceNo = tightenReq.DeviceNo;
                context.MstMsg.TightenStart.DeviceBrand = tightenReq.DeviceBrand;
                context.MstMsg.TightenStart.ProgramNo = tightenReq.ProgramNo;
                context.MstMsg.TightenStart.SortNo = tightenReq.SortNo;
                context.MstMsg.TightenStart.Flag = new RequestFlagBuilder(context.MstMsg.TightenStart.Flag).Req(true).Build();

                tightenReqs.Remove(tightenReq);
                //发送日志消息
                await _mediator.Publish(new UILogNotification(new LogMessage
                {
                    Content = $"[拧紧开始]发起请求，{JsonSerializer.Serialize(tightenReq)}",
                    Level = LogLevel.Information,
                    Timestamp = DateTime.Now
                }));
            }

            //PLC应答了上位机发起拧紧请求，关闭请求
            if (mstReq && devAck)
            {
                var deviceNo = context.MstMsg.TightenStart.DeviceNo;
                var programNo = context.MstMsg.TightenStart.ProgramNo;

                //更新Pending消息
                context.MstMsg.TightenStart.DeviceNo = default;
                context.MstMsg.TightenStart.DeviceBrand = default;
                context.MstMsg.TightenStart.ProgramNo = default;
                context.MstMsg.TightenStart.SortNo = default;
                context.MstMsg.TightenStart.Flag = new RequestFlagBuilder(context.MstMsg.TightenStart.Flag).Req(false).Build();

                //发送日志消息
                await _mediator.Publish(new UILogNotification(new LogMessage
                {
                    Content = $"[拧紧开始]完成请求",
                    Level = LogLevel.Information,
                    Timestamp = DateTime.Now
                }));

                ////发送异常报警信息
                //if (devNG)
                //{
                //    await _mediator.Publish(new AlarmSYSNotification()
                //    {
                //        Code = AlarmCode.拧紧枪错误,
                //        Name = AlarmCode.拧紧使能NG.ToString(),
                //        Module = "拧紧使能NG",
                //        PackCode = string.Empty,
                //        DeviceNo = deviceNo.ToString(),
                //        Description = $"拧紧使能NG，请复位后重试\r\n枪号:{deviceNo}, 程序号:{programNo}"
                //    });
                //    return;
                //}

            }

            await next(context);
        }
    }
}
