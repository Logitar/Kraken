using Logitar.EventSourcing;
using Logitar.Kraken.Contracts;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.Core.Users;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;

internal class UserQuerier : IUserQuerier
{
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<UserEntity> _users;

  public UserQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenContext context)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _users = context.Users;
  }

  public async Task<UserId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await _users.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : new UserId(new StreamId(streamId));
  }

  public async Task<UserModel> ReadAsync(User user, CancellationToken cancellationToken)
  {
    return await ReadAsync(user.Id, cancellationToken) ?? throw new InvalidOperationException($"The user entity 'StreamId={user.Id}' could not be found.");
  }
  public async Task<UserModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new UserId(id, _applicationContext.RealmId), cancellationToken);
  }
  public async Task<UserModel?> ReadAsync(UserId id, CancellationToken cancellationToken)
  {
    UserEntity? user = await _users.AsNoTracking()
      .Include(x => x.Identifiers)
      .Include(x => x.Roles)
      .WhereRealm(id.RealmId)
      .SingleOrDefaultAsync(x => x.Id == id.EntityId, cancellationToken);

    return user == null ? null : await MapAsync(user, cancellationToken);
  }
  public async Task<UserModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    UserEntity? user = await _users.AsNoTracking()
      .Include(x => x.Identifiers)
      .Include(x => x.Roles)
      .WhereRealm(_applicationContext.RealmId)
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    return user == null ? null : await MapAsync(user, cancellationToken);
  }

  public async Task<IReadOnlyCollection<UserModel>> ReadAsync(IEmail email, CancellationToken cancellationToken)
  {
    string emailAddressNormalized = Helper.Normalize(email);

    UserEntity[] users = await _users.AsNoTracking()
      .Include(x => x.Identifiers)
      .Include(x => x.Roles)
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => x.EmailAddressNormalized == emailAddressNormalized)
      .ToArrayAsync(cancellationToken);

    return await MapAsync(users, cancellationToken);
  }

  public async Task<UserModel?> ReadAsync(CustomIdentifierModel customIdentifier, CancellationToken cancellationToken)
  {
    string key = customIdentifier.Key.Trim();
    string value = customIdentifier.Value.Trim();

    UserEntity? user = await _users.AsNoTracking()
      .Include(x => x.Identifiers)
      .Include(x => x.Roles)
      .WhereRealm(_applicationContext.RealmId)
      .SingleOrDefaultAsync(x => x.Identifiers.Any(i => i.Key == key && i.Value == value), cancellationToken);

    return user == null ? null : await MapAsync(user, cancellationToken);
  }

  public Task<SearchResults<UserModel>> SearchAsync(SearchUsersPayload payload, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  private async Task<UserModel> MapAsync(UserEntity user, CancellationToken cancellationToken)
  {
    return (await MapAsync([user], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<UserModel>> MapAsync(IEnumerable<UserEntity> users, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = users.SelectMany(user => user.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return users.Select(user => mapper.ToUser(user, _applicationContext.Realm)).ToArray();
  }
}
