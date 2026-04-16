using System.Threading;
using System.Threading.Tasks;

using Ctp0600P.Client.Protocols;

using MediatR;

namespace Ctp0600P.Client.MessageHandler;

public class AnyLoadMessageHandler : INotificationHandler<AnyLoadRequest>
{
    private readonly IMediator _mediator;
    public AnyLoadMessageHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task Handle(AnyLoadRequest request, CancellationToken cancellationToken)
    {
        App.AnyLoadRequestSubject.OnNext(request);
        return Task.CompletedTask;
    }
}