using Logitar.Kraken.Core.Realms.Events;

namespace Logitar.Kraken.Core.Realms;

internal class RealmManager : IRealmManager
{
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;

  public RealmManager(IRealmQuerier realmQuerier, IRealmRepository realmRepository)
  {
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
  }

  public async Task SaveAsync(Realm realm, CancellationToken cancellationToken)
  {
    bool hasUniqueSlugChanged = realm.Changes.Any(change => change is RealmCreated || change is RealmUniqueSlugChanged);
    if (hasUniqueSlugChanged)
    {
      RealmId? conflictId = await _realmQuerier.FindIdAsync(realm.UniqueSlug, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(realm.Id))
      {
        throw new UniqueSlugAlreadyUsedException(realm, conflictId.Value);
      }
    }

    await _realmRepository.SaveAsync(realm, cancellationToken);
  }
}
