using AsZero.Core.Services.Messages;
using Automatic.Protocols.ModuleInBox.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using StdUnit.Sharp7.Options;

namespace Automatic.Protocols.ModuleInBox.Middlewares.Common
{
    public abstract class HandlePlcRequestMiddlewareBase<TIncoming, TPending, TArgs, TOk, TErr> :
        HandleIncomingRequestMiddlewareBase<DevMsg, MstMsg, ScanContext, ModuleInBoxScanner, TIncoming, TPending, TArgs, TOk, TErr>
    {
        protected readonly IMediator _mediator;

        protected HandlePlcRequestMiddlewareBase(ModuleInBoxFlusher flusher, ILogger<HandlePlcRequestMiddlewareBase<TIncoming, TPending, TArgs, TOk, TErr>> logger, IMediator mediator) : base(flusher, logger)
        {
            _mediator = mediator;
        }

        public override async Task RecordLogAsync(LogLevel level, string logTemplate, params object[] args)
        {
            logTemplate = DealMsgByStation + logTemplate;

            var logmsg = string.Format(logTemplate, args);
            var notification = level < LogLevel.Warning ?
                new UILogNotification(new LogMessage
                {
                    EventSource = PlcName,
                    EventGroup = ProcName,
                    Level = level,
                    Content = logmsg,
                    Timestamp = DateTime.Now,
                }) :
                new UILogNotification(new AlarmMessage
                {
                    Level = level,
                    EventSource = PlcName,
                    EventGroup = ProcName,
                    Content = logmsg,
                    Timestamp = DateTime.Now,
                });
            await _mediator.Publish(notification);
        }

        internal string DealMsgByStation => $"[模组入箱工位] ";

    }
}
