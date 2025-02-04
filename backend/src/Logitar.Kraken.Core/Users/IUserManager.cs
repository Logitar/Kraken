using Logitar.EventSourcing;

namespace Logitar.Kraken.Core.Users;

public interface IUserManager // ISSUE #50: https://github.com/Logitar/Kraken/issues/50
{
  Task<User> FindAsync(string user, string propertyName, bool includeId, CancellationToken cancellationToken = default);
  Task SaveAsync(User user, ActorId? actorId = null, CancellationToken cancellationToken = default);
}
