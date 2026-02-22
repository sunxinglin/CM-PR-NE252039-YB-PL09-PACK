using Itminus.Middlewares;

using Microsoft.Extensions.Logging;

namespace Automatic.Protocols.PressureStripPressurize.Middlewares.Common
{
    public class FlushPendingMiddleware : IWorkMiddleware<ScanContext>
    {
        private readonly ILogger<FlushPendingMiddleware> _logger;
        private readonly PressureStripFlusher _flusher;

        public FlushPendingMiddleware(ILogger<FlushPendingMiddleware> logger, PressureStripFlusher flusher)
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

}
