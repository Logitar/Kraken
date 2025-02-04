using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Contracts.Senders;
using MediatR;

namespace Logitar.Kraken.Core.Senders.Queries;

internal record SearchSendersQuery(SearchSendersPayload Payload) : Activity, IRequest<SearchResults<SenderModel>>;

internal class SearchSendersQueryHandler : IRequestHandler<SearchSendersQuery, SearchResults<SenderModel>>
{
  private readonly ISenderQuerier _senderQuerier;

  public SearchSendersQueryHandler(ISenderQuerier senderQuerier)
  {
    _senderQuerier = senderQuerier;
  }

  public async Task<SearchResults<SenderModel>> Handle(SearchSendersQuery query, CancellationToken cancellationToken)
  {
    return await _senderQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
