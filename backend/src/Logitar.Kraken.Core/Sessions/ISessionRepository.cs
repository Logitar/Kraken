namespace Logitar.Kraken.Core.Sessions;

public interface ISessionRepository
{
  Task<Session?> LoadAsync(SessionId id, CancellationToken cancellationToken = default);
  Task<Session?> LoadAsync(SessionId id, long? version, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<Session>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Session>> LoadAsync(IEnumerable<SessionId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(Session session, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Session> sessions, CancellationToken cancellationToken = default);
}
