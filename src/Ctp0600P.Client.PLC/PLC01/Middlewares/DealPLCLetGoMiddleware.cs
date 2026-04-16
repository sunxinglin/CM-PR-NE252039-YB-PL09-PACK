using Ctp0600P.Client.PLC.Common;
using Ctp0600P.Client.PLC.PLC01.Models.Notifications;

using Itminus.Middlewares;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Ctp0600P.Client.PLC.PLC01.Middlewares;

public class DealPLCLetGoMiddleware : IWorkMiddleware<ScanContext>
{
    private readonly IMediator _mediator;

    public DealPLCLetGoMiddleware(ILogger<DealPLCLetGoMiddleware> logger, IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task InvokeAsync(ScanContext context, WorkDelegate<ScanContext> next)
    {
        var letGoReq = context.DevMsg.Status.HasFlag(DevMsg_GeneralStatus.LetGo);
        var letGoConfirm = context.MstMsg.Status.HasFlag(MstMsg_GeneralStatus.LetGoConfirm);

        //处理请求
        if (letGoReq && !letGoConfirm)
        {
            //发送放行消息
            await _mediator.Publish(new StationPLCStatusNotification { LetGo = true });

            //消息发送出去后，就确认消息
            context.MstMsg.Status = new MstMsg_GeneralStatusBuilder(context.MstMsg.Status)
                .SetLetGoConfirmOnOff(true)
                .Build();
        }

        //重置请求
        if (!letGoReq && letGoConfirm)
        {
            context.MstMsg.Status = new MstMsg_GeneralStatusBuilder(context.MstMsg.Status)
                .SetLetGoConfirmOnOff(false)
                .Build();
        }

        await next(context);
    }
}