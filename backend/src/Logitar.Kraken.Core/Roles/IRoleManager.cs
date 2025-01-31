namespace Logitar.Kraken.Core.Roles;

public interface IRoleManager
{
  Task SaveAsync(Role role, CancellationToken cancellationToken = default);
}
