using Logitar.EventSourcing;

namespace Logitar.Kraken.Core.Users;

internal class UserManager : IUserManager
{
  // ISSUE #50: https://github.com/Logitar/Kraken/issues/50

  public Task<User> FindAsync(string user, string propertyName, bool includeId, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public Task SaveAsync(User user, ActorId? actorId, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
