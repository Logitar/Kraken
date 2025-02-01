using Logitar.Kraken.Contracts.ApiKeys;
using Logitar.Kraken.Contracts.Search;
using MediatR;

namespace Logitar.Kraken.Core.ApiKeys.Queries;

public record SearchApiKeysQuery(SearchApiKeysPayload Payload) : Activity, IRequest<SearchResults<ApiKeyModel>>;

internal class SearchApiKeysQueryHandler : IRequestHandler<SearchApiKeysQuery, SearchResults<ApiKeyModel>>
{
  private readonly IApiKeyQuerier _apiKeyQuerier;

  public SearchApiKeysQueryHandler(IApiKeyQuerier apiKeyQuerier)
  {
    _apiKeyQuerier = apiKeyQuerier;
  }

  public async Task<SearchResults<ApiKeyModel>> Handle(SearchApiKeysQuery query, CancellationToken cancellationToken)
  {
    return await _apiKeyQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
