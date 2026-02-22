using Ctp0600P.Client.PLC.Common;
using Ctp0600P.Client.PLC.PLC01.Models.Notifications;
using Itminus.Middlewares;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ctp0600P.Client.PLC.PLC01.Middlewares
{
    public class DealPLCAlarmResetMiddleware : IWorkMiddleware<ScanContext>
    {
        private readonly IMediator _mediator;

        public DealPLCAlarmResetMiddleware(ILogger<DealPLCAlarmResetMiddleware> logger, IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task InvokeAsync(ScanContext context, WorkDelegate<ScanContext> next)
        {
            var resetReq = context.DevMsg.Status.HasFlag(DevMsg_GeneralStatus.Reset);
            var resetConfirm = context.MstMsg.Status.HasFlag(MstMsg_GeneralStatus.ResetConfirm);

            //处理请求
            if (resetReq && !resetConfirm)
            {
                //发送报警复位消息
                await _mediator.Publish(new StationPLCStatusNotification { Reset = true });

                //消息发送出去后，就确认消息
                context.MstMsg.Status = new MstMsg_GeneralStatusBuilder(context.MstMsg.Status)
                    .SetResetComfirmOnOff(true)
                    .Build();

            }

            //重置请求
            if (!resetReq && resetConfirm)
            {
                context.MstMsg.Status = new MstMsg_GeneralStatusBuilder(context.MstMsg.Status)
                    .SetResetComfirmOnOff(false)
                    .Build();
            }

            await next(context);
        }
    }
}
