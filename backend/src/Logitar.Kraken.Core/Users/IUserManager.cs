namespace Logitar.Kraken.Core.Users;

public interface IUserManager
{
  Task SaveAsync(User user, CancellationToken cancellationToken = default);
}
