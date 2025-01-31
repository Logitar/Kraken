namespace Logitar.Kraken.Core.Roles;

public interface IRoleRepository
{
  Task<Role?> LoadAsync(RoleId id, CancellationToken cancellationToken = default);
  Task<Role?> LoadAsync(RoleId id, long? version, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Role>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Role>> LoadAsync(IEnumerable<RoleId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(Role role, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Role> roles, CancellationToken cancellationToken = default);
}
