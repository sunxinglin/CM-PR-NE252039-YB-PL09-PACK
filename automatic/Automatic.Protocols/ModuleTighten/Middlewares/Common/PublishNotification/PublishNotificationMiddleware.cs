using Itminus.Middlewares;
using MediatR;

namespace Automatic.Protocols.ModuleTighten.Middlewares.Common.PublishNotification
{
    public class PublishNotificationMiddleware : IWorkMiddleware<ScanContext>
    {
        private readonly IMediator _mediator;

        public PublishNotificationMiddleware(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task InvokeAsync(ScanContext context, WorkDelegate<ScanContext> next)
        {
            try
            {
                await _mediator.Publish(new ScanContextNotification(context));
            }
            finally
            {
                await next(context);
            }
        }
    }
}
