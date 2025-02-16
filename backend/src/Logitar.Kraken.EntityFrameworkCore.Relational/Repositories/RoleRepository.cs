using Logitar.EventSourcing;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Roles;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Repositories;

internal class RoleRepository : Repository, IRoleRepository
{
  private readonly DbSet<RoleEntity> _roles;

  public RoleRepository(KrakenContext context, IEventStore eventStore) : base(eventStore)
  {
    _roles = context.Roles;
  }

  public async Task<Role?> LoadAsync(RoleId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted: null, cancellationToken);
  }
  public async Task<Role?> LoadAsync(RoleId id, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted, cancellationToken);
  }
  public async Task<Role?> LoadAsync(RoleId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version, isDeleted: null, cancellationToken);
  }
  public async Task<Role?> LoadAsync(RoleId id, long? version, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<Role>(id.StreamId, version, isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Role>> LoadAsync(CancellationToken cancellationToken)
  {
    return await LoadAsync<Role>(cancellationToken);
  }
  public async Task<IReadOnlyCollection<Role>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<Role>(isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Role>> LoadAsync(IEnumerable<RoleId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync(ids, isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Role>> LoadAsync(IEnumerable<RoleId> ids, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<Role>(ids.Select(id => id.StreamId), isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Role>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken)
  {
    string[] streamIds = await _roles.AsNoTracking()
      .WhereRealm(realmId)
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync(streamIds.Select(streamId => new RoleId(new StreamId(streamId))), cancellationToken);
  }
  public async Task<Role?> LoadAsync(RealmId? realmId, UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await _roles.AsNoTracking()
      .WhereRealm(realmId)
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId == null ? null : await LoadAsync(new RoleId(new StreamId(streamId)), cancellationToken);
  }

  public async Task SaveAsync(Role role, CancellationToken cancellationToken)
  {
    await base.SaveAsync(role, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Role> roles, CancellationToken cancellationToken)
  {
    await base.SaveAsync(roles, cancellationToken);
  }
}
