using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Repositories;

internal class RealmRepository : Repository, IRealmRepository
{
  public RealmRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Realm?> LoadAsync(RealmId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted: null, cancellationToken);
  }
  public async Task<Realm?> LoadAsync(RealmId id, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted, cancellationToken);
  }
  public async Task<Realm?> LoadAsync(RealmId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version, isDeleted: null, cancellationToken);
  }
  public async Task<Realm?> LoadAsync(RealmId id, long? version, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<Realm>(id.StreamId, version, isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Realm>> LoadAsync(CancellationToken cancellationToken)
  {
    return await LoadAsync<Realm>(cancellationToken);
  }
  public async Task<IReadOnlyCollection<Realm>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<Realm>(isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Realm>> LoadAsync(IEnumerable<RealmId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync(ids, isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Realm>> LoadAsync(IEnumerable<RealmId> ids, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<Realm>(ids.Select(id => id.StreamId), isDeleted, cancellationToken);
  }

  public async Task SaveAsync(Realm realm, CancellationToken cancellationToken)
  {
    await base.SaveAsync(realm, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Realm> realms, CancellationToken cancellationToken)
  {
    await base.SaveAsync(realms, cancellationToken);
  }
}
