using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.Roles;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.Core.Roles;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;

internal class RoleQuerier : IRoleQuerier
{
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<RoleEntity> _roles;

  public RoleQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenContext context)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _roles = context.Roles;
  }

  public async Task<RoleId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await _roles.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : new RoleId(new StreamId(streamId));
  }
  public async Task<IReadOnlyCollection<RoleId>> FindIdsAsync(IEnumerable<string> uniqueNames, CancellationToken cancellationToken)
  {
    HashSet<string> uniqueNameNormalized = uniqueNames.Select(Helper.Normalize).ToHashSet();

    string[] streamIds = await _roles.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .Where(x => uniqueNameNormalized.Contains(x.UniqueNameNormalized))
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return streamIds.Select(streamId => new RoleId(new StreamId(streamId))).ToArray();
  }

  public async Task<RoleModel> ReadAsync(Role role, CancellationToken cancellationToken)
  {
    return await ReadAsync(role.Id, cancellationToken) ?? throw new InvalidOperationException($"The role entity 'StreamId={role.Id}' could not be found.");
  }
  public async Task<RoleModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new RoleId(_applicationContext.RealmId, id), cancellationToken);
  }
  public async Task<RoleModel?> ReadAsync(RoleId id, CancellationToken cancellationToken)
  {
    RoleEntity? role = await _roles.AsNoTracking()
      .WhereRealm(id.RealmId)
      .SingleOrDefaultAsync(x => x.Id == id.EntityId, cancellationToken);

    return role == null ? null : await MapAsync(role, cancellationToken);
  }
  public async Task<RoleModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    RoleEntity? role = await _roles.AsNoTracking()
      .WhereRealm(_applicationContext.RealmId)
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    return role == null ? null : await MapAsync(role, cancellationToken);
  }

  public Task<SearchResults<RoleModel>> SearchAsync(SearchRolesPayload payload, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  private async Task<RoleModel> MapAsync(RoleEntity role, CancellationToken cancellationToken)
  {
    return (await MapAsync([role], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<RoleModel>> MapAsync(IEnumerable<RoleEntity> roles, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = roles.SelectMany(role => role.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return roles.Select(role => mapper.ToRole(role, _applicationContext.Realm)).ToArray();
  }
}
