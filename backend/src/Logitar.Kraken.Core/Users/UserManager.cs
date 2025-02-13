
namespace Logitar.Kraken.Core.Users;

internal class UserManager : IUserManager // TODO(fpion): implement
{
  public Task<User> FindAsync(string user, string propertyName, bool includeId, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public Task SaveAsync(User user, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
