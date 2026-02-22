using Ctp0600P.Client.Protocols;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Ctp0600P.Client.MessageHandler
{
    public class AnyLoadMessageHandler : INotificationHandler<AnyLoadRequest>
    {
        private readonly IMediator _mediator;
        public AnyLoadMessageHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(AnyLoadRequest request, CancellationToken cancellationToken)
        {
            App.AnyLoadRequestSubject.OnNext(request);
        }
    }
}
