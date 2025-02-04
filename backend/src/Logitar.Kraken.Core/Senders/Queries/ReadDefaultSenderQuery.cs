using Logitar.Kraken.Contracts.Senders;
using MediatR;

namespace Logitar.Kraken.Core.Senders.Queries;

internal record ReadDefaultSenderQuery : Activity, IRequest<SenderModel?>; // TODO(fpion): merge with ReadSenderQuery

internal class ReadDefaultSenderQueryHandler : IRequestHandler<ReadDefaultSenderQuery, SenderModel?>
{
  private readonly ISenderQuerier _senderQuerier;

  public ReadDefaultSenderQueryHandler(ISenderQuerier senderQuerier)
  {
    _senderQuerier = senderQuerier;
  }

  public async Task<SenderModel?> Handle(ReadDefaultSenderQuery query, CancellationToken cancellationToken)
  {
    return await _senderQuerier.ReadDefaultAsync(SenderType.Email, cancellationToken);
  }
}
