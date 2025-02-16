using Logitar.Data;
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
  private readonly IQueryHelper _queryHelper;
  private readonly DbSet<UserEntity> _users;

  public UserQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenContext context, IQueryHelper queryHelper)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _queryHelper = queryHelper;
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
  public async Task<UserId?> FindIdAsync(Identifier key, CustomIdentifier value, CancellationToken cancellationToken)
  {
    string? streamId = await _users.AsNoTracking()
      .Include(x => x.Identifiers)
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => x.Identifiers.Any(i => i.Key == key.Value && i.Value == value.Value))
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : new UserId(new StreamId(streamId));
  }
  public async Task<IReadOnlyCollection<UserId>> FindIdsAsync(IEmail email, CancellationToken cancellationToken)
  {
    string emailAddressNormalized = Helper.Normalize(email);

    string[] streamIds = await _users.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => x.EmailAddressNormalized == emailAddressNormalized)
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return streamIds.Select(streamId => new UserId(new StreamId(streamId))).ToArray();
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

  public async Task<SearchResults<UserModel>> SearchAsync(SearchUsersPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _queryHelper.From(Users.Table).SelectAll(Users.Table)
      .WhereRealm(Users.RealmUid, _applicationContext.RealmId)
      .ApplyIdFilter(Users.Id, payload.Ids);
    _queryHelper.ApplyTextSearch(builder, payload.Search, Users.UniqueName, Users.AddressFormatted, Users.EmailAddress, Users.PhoneE164Formatted, Users.FirstName, Users.MiddleName, Users.LastName, Users.Nickname);

    if (payload.HasAuthenticated.HasValue)
    {
      NullOperator @operator = payload.HasAuthenticated.Value ? Operators.IsNotNull() : Operators.IsNull();
      builder.Where(Users.AuthenticatedOn, @operator);
    }
    if (payload.HasPassword.HasValue)
    {
      builder.Where(Users.HasPassword, Operators.IsEqualTo(payload.HasPassword.Value));
    }
    if (payload.IsDisabled.HasValue)
    {
      builder.Where(Users.IsDisabled, Operators.IsEqualTo(payload.IsDisabled.Value));
    }
    if (payload.IsConfirmed.HasValue)
    {
      builder.Where(Users.IsConfirmed, Operators.IsEqualTo(payload.IsConfirmed.Value));
    }
    if (payload.RoleId.HasValue)
    {
      // TODO(fpion): implement
    }

    IQueryable<UserEntity> query = _users.FromQuery(builder).AsNoTracking()
      .Include(x => x.Identifiers)
      .Include(x => x.Roles);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<UserEntity>? ordered = null;
    foreach (UserSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case UserSort.AuthenticatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.AuthenticatedOn) : query.OrderBy(x => x.AuthenticatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.AuthenticatedOn) : ordered.ThenBy(x => x.AuthenticatedOn));
          break;
        case UserSort.Birthdate:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Birthdate) : query.OrderBy(x => x.Birthdate))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Birthdate) : ordered.ThenBy(x => x.Birthdate));
          break;
        case UserSort.CreatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisabledOn) : query.OrderBy(x => x.DisabledOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisabledOn) : ordered.ThenBy(x => x.DisabledOn));
          break;
        case UserSort.DisabledOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisabledOn) : query.OrderBy(x => x.DisabledOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisabledOn) : ordered.ThenBy(x => x.DisabledOn));
          break;
        case UserSort.EmailAddress:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.EmailAddress) : query.OrderBy(x => x.EmailAddress))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.EmailAddress) : ordered.ThenBy(x => x.EmailAddress));
          break;
        case UserSort.FirstName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.FirstName) : query.OrderBy(x => x.FirstName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.FirstName) : ordered.ThenBy(x => x.FirstName));
          break;
        case UserSort.FullName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.FullName) : query.OrderBy(x => x.FullName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.FullName) : ordered.ThenBy(x => x.FullName));
          break;
        case UserSort.LastName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.LastName) : query.OrderBy(x => x.LastName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.LastName) : ordered.ThenBy(x => x.LastName));
          break;
        case UserSort.MiddleName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.MiddleName) : query.OrderBy(x => x.MiddleName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.MiddleName) : ordered.ThenBy(x => x.MiddleName));
          break;
        case UserSort.Nickname:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Nickname) : query.OrderBy(x => x.Nickname))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Nickname) : ordered.ThenBy(x => x.Nickname));
          break;
        case UserSort.PasswordChangedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.PasswordChangedOn) : query.OrderBy(x => x.PasswordChangedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.PasswordChangedOn) : ordered.ThenBy(x => x.PasswordChangedOn));
          break;
        case UserSort.PhoneNumber:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.PhoneE164Formatted) : query.OrderBy(x => x.PhoneE164Formatted))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.PhoneE164Formatted) : ordered.ThenBy(x => x.PhoneE164Formatted));
          break;
        case UserSort.UniqueName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName));
          break;
        case UserSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    UserEntity[] users = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<UserModel> items = await MapAsync(users, cancellationToken);

    return new SearchResults<UserModel>(items, total);
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
