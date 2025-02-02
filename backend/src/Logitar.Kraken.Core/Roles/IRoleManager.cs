using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Roles;

public interface IRoleManager
{
  Task<IReadOnlyDictionary<string, Role>> FindAsync(RealmId? realmId, IEnumerable<string> roles, CancellationToken cancellationToken = default);
  Task SaveAsync(Role role, CancellationToken cancellationToken = default);
}
