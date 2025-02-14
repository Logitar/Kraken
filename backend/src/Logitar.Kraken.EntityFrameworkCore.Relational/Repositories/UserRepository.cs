using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Roles;
using Logitar.Kraken.Core.Users;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Repositories;

internal class UserRepository : Repository, IUserRepository
{
  private readonly DbSet<UserEntity> _users;

  public UserRepository(KrakenContext context, IEventStore eventStore) : base(eventStore)
  {
    _users = context.Users;
  }

  public async Task<User?> LoadAsync(UserId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted: null, cancellationToken);
  }
  public async Task<User?> LoadAsync(UserId id, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted, cancellationToken);
  }
  public async Task<User?> LoadAsync(UserId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version, isDeleted: null, cancellationToken);
  }
  public async Task<User?> LoadAsync(UserId id, long? version, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<User>(id.StreamId, version, isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<User>> LoadAsync(CancellationToken cancellationToken)
  {
    return await LoadAsync<User>(cancellationToken);
  }
  public async Task<IReadOnlyCollection<User>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<User>(isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<User>> LoadAsync(IEnumerable<UserId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync(ids, isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<User>> LoadAsync(IEnumerable<UserId> ids, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<User>(ids.Select(id => id.StreamId), isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<User>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken)
  {
    string[] streamIds = await _users.AsNoTracking()
      .WhereRealm(realmId)
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync(streamIds.Select(streamId => new UserId(new StreamId(streamId))), cancellationToken);
  }
  public async Task<IReadOnlyCollection<User>> LoadAsync(RealmId? realmId, IEmail email, CancellationToken cancellationToken)
  {
    string emailAddressNormalized = Helper.Normalize(email);

    string[] streamIds = await _users.AsNoTracking()
      .WhereRealm(realmId)
      .Where(x => x.EmailAddressNormalized == emailAddressNormalized)
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync(streamIds.Select(streamId => new UserId(new StreamId(streamId))), cancellationToken);
  }

  public async Task<IReadOnlyCollection<User>> LoadAsync(RoleId roleId, CancellationToken cancellationToken)
  {
    string[] streamIds = await _users.AsNoTracking()
      .Include(x => x.Roles)
      .WhereRealm(roleId.RealmId)
      .Where(x => x.Roles.Any(r => r.Id == roleId.EntityId))
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync(streamIds.Select(streamId => new UserId(new StreamId(streamId))), cancellationToken);
  }

  public async Task<User?> LoadAsync(RealmId? realmId, UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await _users.AsNoTracking()
      .WhereRealm(realmId)
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : await LoadAsync(new UserId(new StreamId(streamId)), cancellationToken);
  }
  public async Task<User?> LoadAsync(RealmId? realmId, Identifier key, CustomIdentifier value, CancellationToken cancellationToken)
  {
    string? streamId = await _users.AsNoTracking()
      .Include(x => x.Identifiers)
      .WhereRealm(realmId)
      .Where(x => x.Identifiers.Any(i => i.Key == key.Value && i.Value == value.Value))
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : await LoadAsync(new UserId(new StreamId(streamId)), cancellationToken);
  }

  public async Task SaveAsync(User user, CancellationToken cancellationToken)
  {
    await base.SaveAsync(user, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<User> users, CancellationToken cancellationToken)
  {
    await base.SaveAsync(users, cancellationToken);
  }
}
