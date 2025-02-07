using Logitar.EventSourcing;
using Logitar.EventSourcing.Infrastructure;
using MediatR;

namespace Logitar.Kraken.Infrastructure;

public class EventBus : IEventBus
{
  protected IMediator Mediator { get; }

  public EventBus(IMediator mediator)
  {
    Mediator = mediator;
  }

  public async Task PublishAsync(IEvent @event, CancellationToken cancellationToken)
  {
    await Mediator.Publish(@event, cancellationToken);
  }
}
