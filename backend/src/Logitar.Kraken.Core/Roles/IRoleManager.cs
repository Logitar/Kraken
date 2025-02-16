namespace Logitar.Kraken.Core.Roles;

public interface IRoleManager
{
  Task<IReadOnlyDictionary<string, Role>> FindAsync(IEnumerable<string> roles, string propertyName, CancellationToken cancellationToken = default);
  Task SaveAsync(Role role, CancellationToken cancellationToken = default);
}
