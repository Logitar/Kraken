using Logitar.Kraken.Contracts.Messages;
using MediatR;

namespace Logitar.Kraken.Core.Messages.Queries;

internal record ReadMessageQuery(Guid Id) : Activity, IRequest<MessageModel>;

internal class ReadMessageQueryHandler : IRequestHandler<ReadMessageQuery, MessageModel?>
{
  private readonly IMessageQuerier _messageQuerier;

  public ReadMessageQueryHandler(IMessageQuerier messageQuerier)
  {
    _messageQuerier = messageQuerier;
  }

  public async Task<MessageModel?> Handle(ReadMessageQuery query, CancellationToken cancellationToken)
  {
    return await _messageQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
