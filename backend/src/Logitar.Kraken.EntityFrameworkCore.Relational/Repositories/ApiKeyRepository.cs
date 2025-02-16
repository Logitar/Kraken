using Logitar.EventSourcing;
using Logitar.Kraken.Core.ApiKeys;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Roles;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Repositories;

internal class ApiKeyRepository : Repository, IApiKeyRepository
{
  private readonly DbSet<ApiKeyEntity> _apiKeys;

  public ApiKeyRepository(KrakenContext context, IEventStore eventStore) : base(eventStore)
  {
    _apiKeys = context.ApiKeys;
  }

  public async Task<ApiKey?> LoadAsync(ApiKeyId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted: null, cancellationToken);
  }
  public async Task<ApiKey?> LoadAsync(ApiKeyId id, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted, cancellationToken);
  }
  public async Task<ApiKey?> LoadAsync(ApiKeyId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version, isDeleted: null, cancellationToken);
  }
  public async Task<ApiKey?> LoadAsync(ApiKeyId id, long? version, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<ApiKey>(id.StreamId, version, isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<ApiKey>> LoadAsync(CancellationToken cancellationToken)
  {
    return await LoadAsync<ApiKey>(cancellationToken);
  }
  public async Task<IReadOnlyCollection<ApiKey>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<ApiKey>(isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<ApiKey>> LoadAsync(IEnumerable<ApiKeyId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync(ids, isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<ApiKey>> LoadAsync(IEnumerable<ApiKeyId> ids, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<ApiKey>(ids.Select(id => id.StreamId), isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<ApiKey>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken)
  {
    string[] streamIds = await _apiKeys.AsNoTracking()
      .WhereRealm(realmId)
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync(streamIds.Select(streamId => new ApiKeyId(new StreamId(streamId))), cancellationToken);
  }

  public async Task<IReadOnlyCollection<ApiKey>> LoadAsync(RoleId roleId, CancellationToken cancellationToken)
  {
    string[] streamIds = await _apiKeys.AsNoTracking()
      .Include(x => x.Roles)
      .WhereRealm(roleId.RealmId)
      .Where(x => x.Roles.Any(r => r.Id == roleId.EntityId))
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync(streamIds.Select(streamId => new ApiKeyId(new StreamId(streamId))), cancellationToken);
  }

  public async Task SaveAsync(ApiKey apiKey, CancellationToken cancellationToken)
  {
    await base.SaveAsync(apiKey, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<ApiKey> apiKeys, CancellationToken cancellationToken)
  {
    await base.SaveAsync(apiKeys, cancellationToken);
  }
}
