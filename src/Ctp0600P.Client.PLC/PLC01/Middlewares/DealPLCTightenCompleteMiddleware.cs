using System.Text.Json;

using AsZero.Core.Services.Messages;

using Ctp0600P.Client.PLC.Common;
using Ctp0600P.Client.PLC.PLC01.Models.Datas;
using Ctp0600P.Client.PLC.PLC01.Models.Notifications;

using Itminus.Middlewares;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Ctp0600P.Client.PLC.PLC01.Middlewares;

public class DealPLCTightenCompleteMiddleware : IWorkMiddleware<ScanContext>
{
    private readonly IMediator _mediator;

    public DealPLCTightenCompleteMiddleware(ILogger<DealPLCTightenCompleteMiddleware> logger, IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task InvokeAsync(ScanContext context, WorkDelegate<ScanContext> next)
    {
        var tightenCompleteReq = context.DevMsg.TightenComplete.Flag.HasFlag(RequestFlag.Req);
        var tightenCompleteAck = context.MstMsg.TightenComplete.Flag.HasFlag(ResponseFlag.Ack);
        var tightenCompleteOK = context.MstMsg.TightenComplete.Flag.HasFlag(ResponseFlag.OK);
        var tightenCompleteNG = context.MstMsg.TightenComplete.Flag.HasFlag(ResponseFlag.NG);

        //上位机发起拧紧请求，PLC应答
        if (tightenCompleteReq && !tightenCompleteAck)
        {
            //把完成的消息发送出去
            var notification = new StationPLCTightenResultNotification()
            {
                DeviceNo = context.DevMsg.TightenComplete.DeviceNo,
                DeviceBrand = context.DevMsg.TightenComplete.DeviceBrand,
                TightenResult =
                    context.DevMsg.TightenComplete.TightenResult.TighteningResultFlag
                        .HasFlag(TightenFlag.Tightening_OK),
                PSet = context.DevMsg.TightenComplete.TightenResult.Pset_selected,
                FinalTorque = context.DevMsg.TightenComplete.TightenResult.Final_torque,
                Constant1 = context.DevMsg.TightenComplete.TightenResult.Constant1,
                TorqueTrend = context.DevMsg.TightenComplete.TightenResult.Torque_trend,
                FinalAngle = context.DevMsg.TightenComplete.TightenResult.Final_angle,
                Constant2 = context.DevMsg.TightenComplete.TightenResult.Constant2,
                AngleTrend = context.DevMsg.TightenComplete.TightenResult.Angle_trend,
                TorqueRateMin = context.DevMsg.TightenComplete.TightenResult.TorqueRate_Min,
                TargetTorqueRate = context.DevMsg.TightenComplete.TightenResult.TargetTorqueRate,
                TorqueRateMax = context.DevMsg.TightenComplete.TightenResult.TorqueRate_Max,
                AngleMin = context.DevMsg.TightenComplete.TightenResult.Angle_Min,
                TargetAngle = context.DevMsg.TightenComplete.TightenResult.TargetAngle,
                AngleMax = context.DevMsg.TightenComplete.TightenResult.Angle_Max
            };
            await _mediator.Publish(notification);

            //更新Pending消息
            context.MstMsg.TightenComplete.Flag = new ResponseFlagBuilder(context.MstMsg.TightenComplete.Flag)
                .Ack(true)
                .Build();

            //发送日志
            await _mediator.Publish(new UILogNotification(new LogMessage
            {
                Content = $"[拧紧完成]收到请求，{JsonSerializer.Serialize(notification)}",
                Level = LogLevel.Information,
                Timestamp = DateTime.Now
            }));
        }

        //重置请求
        if (!tightenCompleteReq && tightenCompleteAck)
        {
            context.MstMsg.TightenComplete.Flag = new ResponseFlagBuilder(context.MstMsg.TightenComplete.Flag)
                .Reset()
                .Build();
        }

        await next(context);
    }
}