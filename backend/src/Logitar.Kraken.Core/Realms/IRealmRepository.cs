namespace Logitar.Kraken.Core.Realms;

public interface IRealmRepository
{
  Task<Realm?> LoadAsync(RealmId id, CancellationToken cancellationToken = default);
  Task<Realm?> LoadAsync(RealmId id, long? version, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Realm>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Realm>> LoadAsync(IEnumerable<RealmId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(Realm realm, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Realm> realms, CancellationToken cancellationToken = default);
}
