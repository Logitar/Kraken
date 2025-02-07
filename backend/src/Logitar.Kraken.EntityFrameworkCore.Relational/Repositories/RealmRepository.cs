using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Repositories;

internal class RealmRepository : Repository, IRealmRepository
{
  private readonly DbSet<RealmEntity> _realms;

  public RealmRepository(KrakenContext context, IEventStore eventStore) : base(eventStore)
  {
    _realms = context.Realms;
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
    return await LoadAsync(isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Realm>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Realm>(isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Realm>> LoadAsync(IEnumerable<RealmId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync(ids, isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Realm>> LoadAsync(IEnumerable<RealmId> ids, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Realm>(ids.Select(id => id.StreamId), isDeleted, cancellationToken);
  }

  public async Task<Realm?> LoadAsync(Slug uniqueSlug, CancellationToken cancellationToken)
  {
    string uniqueSlugNormalized = Helper.Normalize(uniqueSlug);

    string? streamId = await _realms.AsNoTracking()
      .Where(x => x.UniqueSlugNormalized == uniqueSlugNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : await LoadAsync<Realm>(new StreamId(streamId), cancellationToken);
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
