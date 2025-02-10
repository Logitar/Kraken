using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Contracts.Sessions;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.Core.Sessions;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;

internal class SessionQuerier : ISessionQuerier
{
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<SessionEntity> _sessions;

  public SessionQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenContext context)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _sessions = context.Sessions;
  }

  public async Task<SessionModel> ReadAsync(Session session, CancellationToken cancellationToken)
  {
    return await ReadAsync(session.Id, cancellationToken) ?? throw new InvalidOperationException($"The session entity 'StreamId={session.Id}' could not be found.");
  }
  public async Task<SessionModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new SessionId(_applicationContext.RealmId, id), cancellationToken);
  }
  public async Task<SessionModel?> ReadAsync(SessionId id, CancellationToken cancellationToken)
  {
    SessionEntity? session = await _sessions.AsNoTracking()
      .WhereRealm(id.RealmId)
      .Include(x => x.User).ThenInclude(x => x!.Identifiers)
      .Include(x => x.User).ThenInclude(x => x!.Roles)
      .SingleOrDefaultAsync(x => x.Id == id.EntityId, cancellationToken);

    return session == null ? null : await MapAsync(session, cancellationToken);
  }

  public Task<SearchResults<SessionModel>> SearchAsync(SearchSessionsPayload payload, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  private async Task<SessionModel> MapAsync(SessionEntity session, CancellationToken cancellationToken)
  {
    return (await MapAsync([session], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<SessionModel>> MapAsync(IEnumerable<SessionEntity> sessions, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = sessions.SelectMany(session => session.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return sessions.Select(session => mapper.ToSession(session, _applicationContext.Realm)).ToArray();
  }
}
