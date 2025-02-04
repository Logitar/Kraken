using Logitar.Kraken.Contracts.Senders;
using MediatR;

namespace Logitar.Kraken.Core.Senders.Queries;

public record ReadSenderQuery(Guid? Id, SenderType? Type) : Activity, IRequest<SenderModel?>;

internal class ReadSenderQueryHandler : IRequestHandler<ReadSenderQuery, SenderModel?>
{
  private readonly ISenderQuerier _senderQuerier;

  public ReadSenderQueryHandler(ISenderQuerier senderQuerier)
  {
    _senderQuerier = senderQuerier;
  }

  public async Task<SenderModel?> Handle(ReadSenderQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, SenderModel> senders = new(capacity: 2);

    if (query.Id.HasValue)
    {
      SenderModel? sender = await _senderQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (sender != null)
      {
        senders[sender.Id] = sender;
      }
    }

    if (query.Type.HasValue)
    {
      SenderModel? sender = await _senderQuerier.ReadDefaultAsync(query.Type.Value, cancellationToken);
      if (sender != null)
      {
        senders[sender.Id] = sender;
      }
    }

    if (senders.Count > 1)
    {
      throw TooManyResultsException<SenderModel>.ExpectedSingle(senders.Count);
    }

    return senders.SingleOrDefault().Value;
  }
}
