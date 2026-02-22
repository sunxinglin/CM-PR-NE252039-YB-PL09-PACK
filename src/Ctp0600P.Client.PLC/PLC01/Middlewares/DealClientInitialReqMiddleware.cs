using AsZero.Core.Services.Messages;
using Ctp0600P.Client.PLC.Common;
using Ctp0600P.Client.PLC.Context;
using Itminus.Middlewares;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ctp0600P.Client.PLC.PLC01.Middlewares
{
    public class DealClientInitialReqMiddleware : IWorkMiddleware<ScanContext>
    {
        private readonly StationPLCContext _stationPLCContext;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public DealClientInitialReqMiddleware(ILogger<DealClientInitialReqMiddleware> logger, IMediator mediator, StationPLCContext stationPLCContext, IConfiguration configuration)
        {
            _stationPLCContext = stationPLCContext;
            _mediator = mediator;
            _configuration = configuration;
        }

        public async Task InvokeAsync(ScanContext context, WorkDelegate<ScanContext> next)
        {
            var mstReq = context.MstMsg.Status.HasFlag(MstMsg_GeneralStatus.InitialReq);
            var devAck = context.DevMsg.Status.HasFlag(DevMsg_GeneralStatus.InitialAck);

            var clientInitialReq = _stationPLCContext.Initial;

            //请求初始化
            if (clientInitialReq && !mstReq && !devAck)
            {
                context.MstMsg.Status = new MstMsg_GeneralStatusBuilder(context.MstMsg.Status).SetInitialReqOnOff(true).Build();

                //发送日志消息
                await _mediator.Publish(new UILogNotification(new LogMessage { Content = $"[初始化]发起请求", Level = LogLevel.Information, Timestamp = DateTime.Now }));
            }

            //PLC应答，客户端关闭请求
            if (mstReq && devAck)
            {
                //清除客户端请求
                _stationPLCContext.StationTightenStartReqs.Clear();
                _stationPLCContext.LetGo = false;

                //清除拧紧开始信号
                context.MstMsg.TightenStart.Flag = new RequestFlagBuilder(context.MstMsg.TightenStart.Flag).RestAll().Build();
                context.MstMsg.TightenStart.DeviceNo = default;
                context.MstMsg.TightenStart.DeviceBrand = default;
                context.MstMsg.TightenStart.ProgramNo = default;

                //清除拧紧完成确认信号
                context.MstMsg.TightenComplete.Flag = new ResponseFlagBuilder(context.MstMsg.TightenComplete.Flag).RestAll().Build();

                //更新Pending消息
                context.MstMsg.Status = new MstMsg_GeneralStatusBuilder(context.MstMsg.Status).SetInitialReqOnOff(false).Build();
                await _mediator.Publish(new UILogNotification(new LogMessage { Content = $"[初始化]完成请求", Level = LogLevel.Information, Timestamp = DateTime.Now }));
            }

            //清除客户端的初始化标识
            _stationPLCContext.Initial = false;

            await next(context);
        }
    }
}
