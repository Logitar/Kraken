﻿using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Roles;

public interface IRoleRepository
{
  Task<Role?> LoadAsync(RoleId id, CancellationToken cancellationToken = default);
  Task<Role?> LoadAsync(RoleId id, bool? isDeleted, CancellationToken cancellationToken = default);
  Task<Role?> LoadAsync(RoleId id, long? version, CancellationToken cancellationToken = default);
  Task<Role?> LoadAsync(RoleId id, long? version, bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Role>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Role>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Role>> LoadAsync(IEnumerable<RoleId> ids, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Role>> LoadAsync(IEnumerable<RoleId> ids, bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Role>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken = default);
  Task<Role?> LoadAsync(RealmId? realmId, UniqueName uniqueName, CancellationToken cancellationToken = default);

  Task SaveAsync(Role role, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Role> roles, CancellationToken cancellationToken = default);
}
