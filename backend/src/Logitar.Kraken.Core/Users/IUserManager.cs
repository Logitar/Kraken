using Logitar.EventSourcing;
using Logitar.Kraken.Core.Settings;

namespace Logitar.Kraken.Core.Users;

public interface IUserManager
{
  Task SaveAsync(IUserSettings userSettings, User user, ActorId? actorId = null, CancellationToken cancellationToken = default);
}
