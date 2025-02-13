namespace Logitar.Kraken.Core.Users;

public interface IUserManager
{
  Task<User> FindAsync(string user, string propertyName, bool includeId = false, CancellationToken cancellationToken = default);
  Task SaveAsync(User user, CancellationToken cancellationToken = default);
}
