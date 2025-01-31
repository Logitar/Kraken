using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Contracts.Search;
using MediatR;

namespace Logitar.Kraken.Core.Realms.Queries;

public record SearchRealmsQuery(SearchRealmsPayload Payload) : Activity, IRequest<SearchResults<RealmModel>>;

internal class SearchRealmsQueryHandler : IRequestHandler<SearchRealmsQuery, SearchResults<RealmModel>>
{
  private readonly IRealmQuerier _realmQuerier;

  public SearchRealmsQueryHandler(IRealmQuerier realmQuerier)
  {
    _realmQuerier = realmQuerier;
  }

  public async Task<SearchResults<RealmModel>> Handle(SearchRealmsQuery query, CancellationToken cancellationToken)
  {
    return await _realmQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
