using Ctp0600P.Client.PLC.Common;
using Ctp0600P.Client.PLC.PLC01.Models.Notifications;
using Itminus.Middlewares;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ctp0600P.Client.PLC.PLC01.Middlewares
{
    public class DealPLCModeMiddleware : IWorkMiddleware<ScanContext>
    {
        private readonly IMediator _mediator;

        public DealPLCModeMiddleware(ILogger<DealPLCModeMiddleware> logger, IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task InvokeAsync(ScanContext context, WorkDelegate<ScanContext> next)
        {
            var autoMode = context.DevMsg.Status.HasFlag(DevMsg_GeneralStatus.Auto);
            var manualMode = context.DevMsg.Status.HasFlag(DevMsg_GeneralStatus.Manual);
            var fan = context.DevMsg.Status.HasFlag(DevMsg_GeneralStatus.Fan);

            //发送自动/手动模式消息
            await _mediator.Publish(new StationPLCStatusNotification { AutoMode = autoMode, ManualMode = manualMode, Fan = fan });

            await next(context);
        }
    }
}
