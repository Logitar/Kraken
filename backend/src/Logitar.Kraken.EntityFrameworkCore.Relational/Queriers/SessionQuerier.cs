using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Kraken.Contracts.Actors;
using Logitar.Kraken.Contracts.Search;
using Logitar.Kraken.Contracts.Sessions;
using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Actors;
using Logitar.Kraken.Core.Sessions;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using Logitar.Kraken.EntityFrameworkCore.Relational.KrakenDb;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Queriers;

internal class SessionQuerier : ISessionQuerier
{
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;
  private readonly IQueryHelper _queryHelper;
  private readonly DbSet<SessionEntity> _sessions;

  public SessionQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenContext context, IQueryHelper queryHelper)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _queryHelper = queryHelper;
    _sessions = context.Sessions;
  }

  public async Task<SessionModel> ReadAsync(Session session, CancellationToken cancellationToken)
  {
    return await ReadAsync(session.Id, cancellationToken) ?? throw new InvalidOperationException($"The session entity 'StreamId={session.Id}' could not be found.");
  }
  public async Task<SessionModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(new SessionId(id, _applicationContext.RealmId), cancellationToken);
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

  public async Task<SearchResults<SessionModel>> SearchAsync(SearchSessionsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _queryHelper.From(Sessions.Table).SelectAll(Sessions.Table)
      .Join(Users.UserId, Sessions.UserId)
      .WhereRealm(Sessions.RealmUid, _applicationContext.RealmId)
      .ApplyIdFilter(Sessions.Id, payload.Ids);
    _queryHelper.ApplyTextSearch(builder, payload.Search);

    if (payload.UserId.HasValue)
    {
      builder.Where(Users.Id, Operators.IsEqualTo(payload.UserId.Value));
    }
    if (payload.IsActive.HasValue)
    {
      builder.Where(Sessions.IsActive, Operators.IsEqualTo(payload.IsActive.Value));
    }
    if (payload.IsPersistent.HasValue)
    {
      builder.Where(Sessions.IsPersistent, Operators.IsEqualTo(payload.IsPersistent.Value));
    }

    IQueryable<SessionEntity> query = _sessions.FromQuery(builder).AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.Identifiers)
      .Include(x => x.User).ThenInclude(x => x!.Roles);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<SessionEntity>? ordered = null;
    foreach (SessionSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case SessionSort.CreatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case SessionSort.SignedOutOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.SignedOutOn) : query.OrderBy(x => x.SignedOutOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.SignedOutOn) : ordered.ThenBy(x => x.SignedOutOn));
          break;
        case SessionSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    SessionEntity[] sessions = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<SessionModel> items = await MapAsync(sessions, cancellationToken);

    return new SearchResults<SessionModel>(items, total);
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
