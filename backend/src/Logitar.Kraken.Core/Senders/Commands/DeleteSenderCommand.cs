using Logitar.Kraken.Contracts.Senders;
using MediatR;

namespace Logitar.Kraken.Core.Senders.Commands;

public record DeleteSenderCommand(Guid Id) : Activity, IRequest<SenderModel?>;

internal class DeleteSenderCommandHandler : IRequestHandler<DeleteSenderCommand, SenderModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public DeleteSenderCommandHandler(IApplicationContext applicationContext, ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    _applicationContext = applicationContext;
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<SenderModel?> Handle(DeleteSenderCommand command, CancellationToken cancellationToken)
  {
    SenderId senderId = new(_applicationContext.RealmId, command.Id);
    Sender? sender = await _senderRepository.LoadAsync(senderId, cancellationToken);
    if (sender == null)
    {
      return null;
    }

    if (sender.IsDefault)
    {
      IReadOnlyCollection<Sender> senders = await _senderRepository.LoadAsync(sender.RealmId, cancellationToken);
      int count = senders.Count(s => !s.Equals(sender) && s.Type == sender.Type);
      if (count > 0)
      {
        throw new CannotDeleteDefaultSenderException(sender);
      }
    }

    SenderModel result = await _senderQuerier.ReadAsync(sender, cancellationToken);

    sender.Delete(_applicationContext.ActorId);
    await _senderRepository.SaveAsync(sender, cancellationToken);

    return result;
  }
}
