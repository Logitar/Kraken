using Logitar.Kraken.Contracts.ApiKeys;
using Logitar.Kraken.Contracts.Search;

namespace Logitar.Kraken.Core.ApiKeys;

public interface IApiKeyQuerier
{
  Task<ApiKeyModel> ReadAsync(ApiKey apikey, CancellationToken cancellationToken = default);
  Task<ApiKeyModel?> ReadAsync(ApiKeyId id, CancellationToken cancellationToken = default);
  Task<ApiKeyModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<ApiKeyModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<ApiKeyModel>> SearchAsync(SearchApiKeysPayload payload, CancellationToken cancellationToken = default);
}
