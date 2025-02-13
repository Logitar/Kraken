using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Users;

namespace Logitar.Kraken.Core.Sessions;

public interface ISessionRepository
{
  Task<Session?> LoadAsync(SessionId id, CancellationToken cancellationToken = default);
  Task<Session?> LoadAsync(SessionId id, bool? isDeleted, CancellationToken cancellationToken = default);
  Task<Session?> LoadAsync(SessionId id, long? version, CancellationToken cancellationToken = default);
  Task<Session?> LoadAsync(SessionId id, long? version, bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Session>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Session>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Session>> LoadAsync(IEnumerable<SessionId> ids, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Session>> LoadAsync(IEnumerable<SessionId> ids, bool? isDeleted, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Session>> LoadActiveAsync(UserId userId, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Session>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Session>> LoadAsync(UserId userId, CancellationToken cancellationToken = default);

  Task SaveAsync(Session session, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Session> sessions, CancellationToken cancellationToken = default);
}
