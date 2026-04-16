using AsZero.Core.Services.Messages;

using Ctp0600P.Client.PLC.PLC01.Models;

using MediatR;

using Microsoft.Extensions.Logging;

using StdUnit.Sharp7.Options;

namespace Ctp0600P.Client.PLC.PLC01.Middlewares.Common;

public abstract class HandlePlcRequestMiddlewareBase<TIncoming, TPending, TArgs, TOk, TErr> :
    HandleIncomingRequestMiddlewareBase<DevMsg, MstMsg, ScanContext, PlcScanner, TIncoming, TPending, TArgs, TOk,
        TErr>
{
    protected readonly IMediator _mediator;

    protected HandlePlcRequestMiddlewareBase(PlcCtrlFlusher flusher,
        ILogger<HandlePlcRequestMiddlewareBase<TIncoming, TPending, TArgs, TOk, TErr>> logger,
        IMediator mediator) : base(flusher, logger)
    {
        _mediator = mediator;
    }

    public override async Task RecordLogAsync(LogLevel level, string logTemplate, params object[] args)
    {
        logTemplate = DealMsgByStation + logTemplate;
        var logmsg = string.Format(logTemplate, args);
        var notification = level < LogLevel.Warning
            ? new UILogNotification(new LogMessage
            {
                Level = level,
                Content = logmsg,
                Timestamp = DateTime.Now,
            })
            : new UILogNotification(new AlarmMessage
            {
                Level = level,
                EventSource = PlcName,
                Content = logmsg,
                Timestamp = DateTime.Now,
            });
        await _mediator.Publish(notification);
    }

    internal string DealMsgByStation => $"[下箱体涂胶工位] ";
}