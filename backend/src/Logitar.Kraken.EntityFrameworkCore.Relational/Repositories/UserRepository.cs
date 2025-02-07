using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Roles;
using Logitar.Kraken.Core.Sessions;
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
    return await LoadAsync(isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<User>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<User>(isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<User>> LoadAsync(IEnumerable<UserId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync(ids, isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<User>> LoadAsync(IEnumerable<UserId> ids, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<User>(ids.Select(id => id.StreamId), isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<User>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken)
  {
    Guid? id = realmId?.ToGuid();

    string[] streamIds = await _users.AsNoTracking()
      .Include(x => x.Realm)
      .Where(x => id.HasValue ? x.Realm!.Id == id.Value : x.RealmId == null)
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync<User>(streamIds.Select(value => new StreamId(value)), cancellationToken);
  }

  public async Task<IReadOnlyCollection<User>> LoadAsync(RealmId? realmId, IEmail email, CancellationToken cancellationToken)
  {
    Guid? id = realmId?.ToGuid();
    string emailAddressNormalized = Helper.Normalize(email);

    string[] streamIds = await _users.AsNoTracking()
      .Include(x => x.Realm)
      .Where(x => (id.HasValue ? x.Realm!.Id == id.Value : x.RealmId == null) && x.EmailAddressNormalized == emailAddressNormalized)
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync<User>(streamIds.Select(value => new StreamId(value)), cancellationToken);
  }

  public async Task<IReadOnlyCollection<User>> LoadAsync(RoleId roleId, CancellationToken cancellationToken)
  {
    Guid? realmId = roleId.RealmId?.ToGuid();
    Guid id = roleId.EntityId;

    string[] streamIds = await _users.AsNoTracking()
      .Include(x => x.Realm)
      .Include(x => x.Roles)
      .Where(x => (realmId.HasValue ? x.Realm!.Id == realmId.Value : x.RealmId == null) && x.Roles.Any(role => role.Id == id))
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync<User>(streamIds.Select(value => new StreamId(value)), cancellationToken);
  }

  public async Task<User?> LoadAsync(RealmId? realmId, Identifier identifierKey, CustomIdentifier identifierValue, CancellationToken cancellationToken)
  {
    Guid? id = realmId?.ToGuid();
    string key = identifierKey.Value;
    string value = identifierValue.Value;

    string? streamId = await _users.AsNoTracking()
      .Include(x => x.Identifiers)
      .Include(x => x.Realm)
      .Where(x => (id.HasValue ? x.Realm!.Id == id.Value : x.RealmId == null) && x.Identifiers.Any(identifier => identifier.Key == key && identifier.Value == value))
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : await LoadAsync<User>(new StreamId(streamId), cancellationToken);
  }

  public async Task<User?> LoadAsync(RealmId? realmId, UniqueName uniqueName, CancellationToken cancellationToken)
  {
    Guid? id = realmId?.ToGuid();
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await _users.AsNoTracking()
      .Include(x => x.Realm)
      .Where(x => (id.HasValue ? x.Realm!.Id == id.Value : x.RealmId == null) && x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : await LoadAsync<User>(new StreamId(streamId), cancellationToken);
  }

  public async Task<User> LoadAsync(Session session, CancellationToken cancellationToken)
  {
    return await LoadAsync(session.UserId, cancellationToken) ?? throw new InvalidOperationException($"The user 'Id={session.UserId}' could not be found.");
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
