using Logitar.EventSourcing;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Users;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Repositories;

internal class OneTimePasswordRepository : Repository, IOneTimePasswordRepository
{
  private readonly DbSet<OneTimePasswordEntity> _oneTimePasswords;

  public OneTimePasswordRepository(KrakenContext context, IEventStore eventStore) : base(eventStore)
  {
    _oneTimePasswords = context.OneTimePasswords;
  }

  public async Task<OneTimePassword?> LoadAsync(OneTimePasswordId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted: null, cancellationToken);
  }
  public async Task<OneTimePassword?> LoadAsync(OneTimePasswordId id, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted, cancellationToken);
  }
  public async Task<OneTimePassword?> LoadAsync(OneTimePasswordId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version, isDeleted: null, cancellationToken);
  }
  public async Task<OneTimePassword?> LoadAsync(OneTimePasswordId id, long? version, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<OneTimePassword>(id.StreamId, version, isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<OneTimePassword>> LoadAsync(CancellationToken cancellationToken)
  {
    return await LoadAsync<OneTimePassword>(cancellationToken);
  }
  public async Task<IReadOnlyCollection<OneTimePassword>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<OneTimePassword>(isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<OneTimePassword>> LoadAsync(IEnumerable<OneTimePasswordId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync(ids, isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<OneTimePassword>> LoadAsync(IEnumerable<OneTimePasswordId> ids, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<OneTimePassword>(ids.Select(id => id.StreamId), isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<OneTimePassword>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken)
  {
    string[] streamIds = await _oneTimePasswords.AsNoTracking()
      .WhereRealm(realmId)
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync(streamIds.Select(streamId => new OneTimePasswordId(new StreamId(streamId))), cancellationToken);
  }

  public async Task<IReadOnlyCollection<OneTimePassword>> LoadAsync(UserId userId, CancellationToken cancellationToken)
  {
    string[] streamIds = await _oneTimePasswords.AsNoTracking()
      .WhereRealm(userId.RealmId)
      .Where(x => x.User!.Id == userId.EntityId)
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync(streamIds.Select(streamId => new OneTimePasswordId(new StreamId(streamId))), cancellationToken);
  }

  public async Task SaveAsync(OneTimePassword oneTimePassword, CancellationToken cancellationToken)
  {
    await base.SaveAsync(oneTimePassword, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<OneTimePassword> oneTimePasswords, CancellationToken cancellationToken)
  {
    await base.SaveAsync(oneTimePasswords, cancellationToken);
  }
}
