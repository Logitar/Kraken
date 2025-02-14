using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Sessions;
using Logitar.Kraken.Core.Users;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Repositories;

internal class SessionRepository : Repository, ISessionRepository
{
  private readonly DbSet<SessionEntity> _sessions;

  public SessionRepository(KrakenContext context, IEventStore eventStore) : base(eventStore)
  {
    _sessions = context.Sessions;
  }

  public async Task<Session?> LoadAsync(SessionId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted: null, cancellationToken);
  }
  public async Task<Session?> LoadAsync(SessionId id, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, isDeleted, cancellationToken);
  }
  public async Task<Session?> LoadAsync(SessionId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version, isDeleted: null, cancellationToken);
  }
  public async Task<Session?> LoadAsync(SessionId id, long? version, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<Session>(id.StreamId, version, isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Session>> LoadAsync(CancellationToken cancellationToken)
  {
    return await LoadAsync<Session>(cancellationToken);
  }
  public async Task<IReadOnlyCollection<Session>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<Session>(isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Session>> LoadAsync(IEnumerable<SessionId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync(ids, isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Session>> LoadAsync(IEnumerable<SessionId> ids, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await LoadAsync<Session>(ids.Select(id => id.StreamId), isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Session>> LoadActiveAsync(UserId userId, CancellationToken cancellationToken)
  {
    string[] streamIds = await _sessions.AsNoTracking()
      .WhereRealm(userId.RealmId)
      .Where(x => x.User!.Id == userId.EntityId && x.IsActive)
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync(streamIds.Select(streamId => new SessionId(new StreamId(streamId))), cancellationToken);
  }

  public async Task<IReadOnlyCollection<Session>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken)
  {
    string[] streamIds = await _sessions.AsNoTracking()
      .WhereRealm(realmId)
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync(streamIds.Select(streamId => new SessionId(new StreamId(streamId))), cancellationToken);
  }

  public async Task<IReadOnlyCollection<Session>> LoadAsync(UserId userId, CancellationToken cancellationToken)
  {
    string[] streamIds = await _sessions.AsNoTracking()
      .WhereRealm(userId.RealmId)
      .Where(x => x.User!.Id == userId.EntityId)
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync(streamIds.Select(streamId => new SessionId(new StreamId(streamId))), cancellationToken);
  }

  public async Task SaveAsync(Session session, CancellationToken cancellationToken)
  {
    await base.SaveAsync(session, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Session> sessions, CancellationToken cancellationToken)
  {
    await base.SaveAsync(sessions, cancellationToken);
  }
}
