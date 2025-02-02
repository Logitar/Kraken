using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Users;

public interface IUserManager // TODO(fpion): implement
{
  Task<User> FindAsync(RealmId? realmId, string user, string propertyName, bool includeId, CancellationToken cancellationToken = default);
  Task SaveAsync(User user, ActorId? actorId = null, CancellationToken cancellationToken = default);
}
