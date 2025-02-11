using Logitar.Kraken.Contracts.Realms;

namespace Logitar.Kraken.Core.Realms;

public interface IRealmQuerier
{
  Task<RealmId?> FindIdAsync(Slug uniqueSlug, CancellationToken cancellationToken = default);

  Task<RealmModel> ReadAsync(Realm realm, CancellationToken cancellationToken = default);
  Task<RealmModel?> ReadAsync(RealmId id, CancellationToken cancellationToken = default);
  Task<RealmModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RealmModel?> ReadAsync(string uniqueSlug, CancellationToken cancellationToken = default);
}
