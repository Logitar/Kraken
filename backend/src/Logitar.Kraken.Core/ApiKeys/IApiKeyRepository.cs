namespace Logitar.Kraken.Core.ApiKeys;

public interface IApiKeyRepository
{
  Task<ApiKey?> LoadAsync(ApiKeyId id, CancellationToken cancellationToken = default);
  Task<ApiKey?> LoadAsync(ApiKeyId id, long? version, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<ApiKey>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<ApiKey>> LoadAsync(IEnumerable<ApiKeyId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(ApiKey apiKey, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<ApiKey> apiKeys, CancellationToken cancellationToken = default);
}
