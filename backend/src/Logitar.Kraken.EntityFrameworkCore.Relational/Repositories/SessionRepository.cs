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
    return await LoadAsync(isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Session>> LoadAsync(bool? isDeleted, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Session>(isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Session>> LoadAsync(IEnumerable<SessionId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync(ids, isDeleted: null, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Session>> LoadAsync(IEnumerable<SessionId> ids, bool? isDeleted, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Session>(ids.Select(id => id.StreamId), isDeleted, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Session>> LoadActiveAsync(UserId userId, CancellationToken cancellationToken)
  {
    Guid? realmId = userId.RealmId?.ToGuid();
    Guid id = userId.EntityId;

    string[] streamIds = await _sessions.AsNoTracking()
      .Include(x => x.Realm)
      .Include(x => x.User)
      .Where(x => (realmId.HasValue ? x.Realm!.Id == realmId.Value : x.RealmId == null) && x.User!.Id == id && x.IsActive)

      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync<Session>(streamIds.Select(value => new StreamId(value)), cancellationToken);
  }

  public async Task<IReadOnlyCollection<Session>> LoadAsync(RealmId? realmId, CancellationToken cancellationToken)
  {
    Guid? id = realmId?.ToGuid();

    string[] streamIds = await _sessions.AsNoTracking()
      .Include(x => x.Realm)
      .Where(x => id.HasValue ? x.Realm!.Id == id.Value : x.RealmId == null)
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync<Session>(streamIds.Select(value => new StreamId(value)), cancellationToken);
  }

  public async Task<IReadOnlyCollection<Session>> LoadAsync(UserId userId, CancellationToken cancellationToken)
  {
    Guid? realmId = userId.RealmId?.ToGuid();
    Guid id = userId.EntityId;

    string[] streamIds = await _sessions.AsNoTracking()
      .Include(x => x.Realm)
      .Include(x => x.User)
      .Where(x => (realmId.HasValue ? x.Realm!.Id == realmId.Value : x.RealmId == null) && x.User!.Id == id)
      .Select(x => x.StreamId)
      .Distinct()
      .ToArrayAsync(cancellationToken);

    return await LoadAsync<Session>(streamIds.Select(value => new StreamId(value)), cancellationToken);
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
