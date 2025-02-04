using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Senders;
using MediatR;

namespace Logitar.Kraken.Core.Senders.Commands;

public record SetDefaultSenderCommand(Guid Id) : Activity, IRequest<SenderModel?>;

internal class SetDefaultSenderCommandHandler : IRequestHandler<SetDefaultSenderCommand, SenderModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public SetDefaultSenderCommandHandler(IApplicationContext applicationContext, ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    _applicationContext = applicationContext;
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<SenderModel?> Handle(SetDefaultSenderCommand command, CancellationToken cancellationToken)
  {
    SenderId senderId = new(_applicationContext.RealmId, command.Id);
    Sender? sender = await _senderRepository.LoadAsync(senderId, cancellationToken);
    if (sender == null)
    {
      return null;
    }

    if (!sender.IsDefault)
    {
      List<Sender> senders = new(capacity: 2);

      ActorId? actorId = _applicationContext.ActorId;

      Sender? @default = await _senderRepository.LoadDefaultAsync(sender.RealmId, sender.Type, cancellationToken);
      if (@default != null)
      {
        @default.SetDefault(isDefault: false, actorId);
        senders.Add(@default);
      }

      sender.SetDefault(isDefault: true, actorId);
      senders.Add(sender);

      await _senderRepository.SaveAsync(senders, cancellationToken);
    }

    return await _senderQuerier.ReadAsync(sender, cancellationToken);
  }
}
