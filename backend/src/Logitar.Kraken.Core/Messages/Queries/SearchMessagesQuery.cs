using Logitar.Kraken.Contracts.Messages;
using Logitar.Kraken.Contracts.Search;
using MediatR;

namespace Logitar.Kraken.Core.Messages.Queries;

internal record SearchMessagesQuery(SearchMessagesPayload Payload) : Activity, IRequest<SearchResults<MessageModel>>;

internal class SearchMessagesQueryHandler : IRequestHandler<SearchMessagesQuery, SearchResults<MessageModel>>
{
  private readonly IMessageQuerier _messageQuerier;

  public SearchMessagesQueryHandler(IMessageQuerier messageQuerier)
  {
    _messageQuerier = messageQuerier;
  }

  public async Task<SearchResults<MessageModel>> Handle(SearchMessagesQuery query, CancellationToken cancellationToken)
  {
    return await _messageQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
