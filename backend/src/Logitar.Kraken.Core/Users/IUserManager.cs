using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Settings;

namespace Logitar.Kraken.Core.Users;

public interface IUserManager // TODO(fpion): implement
{
  Task<User> FindAsync(IUserSettings settings, RealmId? realmId, string user, string propertyName, bool includeId, CancellationToken cancellationToken = default);
  Task SaveAsync(IUserSettings settings, User user, ActorId? actorId = null, CancellationToken cancellationToken = default);
}
