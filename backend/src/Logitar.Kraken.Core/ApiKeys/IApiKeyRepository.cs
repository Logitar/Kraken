using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Roles;

namespace Logitar.Kraken.Core.ApiKeys;

public interface IApiKeyRepository
{
  Task<ApiKey?> LoadAsync(ApiKeyId id, CancellationToken cancellationToken = default);
  Task<ApiKey?> LoadAsync(ApiKeyId id, long? version, CancellationToken cancellationToken = default);
  Task<ApiKey?> LoadAsync(ApiKeyId id, bool? isDeleted, CancellationToken cancellationToken = default);
  Task<ApiKey?> LoadAsync(ApiKeyId id, bool? isDeleted, long? version, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<ApiKey>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<ApiKey>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<ApiKey>> LoadAsync(IEnumerable<ApiKeyId> ids, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<ApiKey>> LoadAsync(IEnumerable<ApiKeyId> ids, bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<ApiKey>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<ApiKey>> LoadAsync(RoleId roleId, CancellationToken cancellationToken = default);

  Task SaveAsync(ApiKey apiKey, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<ApiKey> apiKeys, CancellationToken cancellationToken = default);
}
