using Itminus.Middlewares;

using Microsoft.Extensions.Logging;

namespace Ctp0600P.Client.PLC.PLC01.Middlewares.Common;

/// <summary>
/// MES写数据到PLC
/// </summary>
public class FlushPendingMiddleware : IWorkMiddleware<ScanContext>
{
    private readonly ILogger<FlushPendingMiddleware> _logger;
    private readonly PlcCtrlFlusher _flusher;

    public FlushPendingMiddleware(ILogger<FlushPendingMiddleware> logger, PlcCtrlFlusher flusher)
    {
        _logger = logger;
        _flusher = flusher;
    }

    public async Task InvokeAsync(ScanContext context, WorkDelegate<ScanContext> next)
    {
        try
        {
            await _flusher.FlushAsync(context.MstMsg);
        }
        finally
        {
            await next(context);
        }
    }
}