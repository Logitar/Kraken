using Logitar.EventSourcing;
using Logitar.Kraken.Core.Sessions.Events;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Handlers;

internal class SessionEvents : INotificationHandler<SessionCreated>,
  INotificationHandler<SessionDeleted>,
  INotificationHandler<SessionRenewed>,
  INotificationHandler<SessionSignedOut>,
  INotificationHandler<SessionUpdated>
{
  private readonly KrakenContext _context;
  private readonly ILogger<SessionEvents> _logger;

  public SessionEvents(KrakenContext context, ILogger<SessionEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task Handle(SessionCreated @event, CancellationToken cancellationToken)
  {
    SessionEntity? session = await FindAsync(@event, cancellationToken);
    if (session == null)
    {
      UserEntity user = await _context.FindUserAsync(@event.UserId, cancellationToken);

      session = new(user, @event);

      _context.Sessions.Add(session);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
    else
    {
      _logger.LogWarning("{Event}: the session entity 'StreamId={Id}' already exists.", @event.GetType().Name, @event.StreamId);
    }
  }

  public async Task Handle(SessionDeleted @event, CancellationToken cancellationToken)
  {
    SessionEntity? session = await FindAsync(@event, cancellationToken);
    if (session == null)
    {
      _logger.LogWarning("{Event}: the session entity 'StreamId={StreamId}' is already deleted.", @event.GetType().Name, @event.StreamId);
    }
    else
    {
      _context.Sessions.Remove(session);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(SessionRenewed @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    SessionEntity? session = await FindAsync(@event, cancellationToken);
    if (session == null || session.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The session entity was expected to be at version {expectedVersion}, but was found at version {session?.Version ?? 0}.");
    }
    else if (session.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the session entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        session.Version,
        expectedVersion);
    }
    else
    {
      session.Renew(@event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(SessionSignedOut @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    SessionEntity? session = await FindAsync(@event, cancellationToken);
    if (session == null || session.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The session entity was expected to be at version {expectedVersion}, but was found at version {session?.Version ?? 0}.");
    }
    else if (session.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the session entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        session.Version,
        expectedVersion);
    }
    else
    {
      session.SignOut(@event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(SessionUpdated @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    SessionEntity? session = await FindAsync(@event, cancellationToken);
    if (session == null || session.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The session entity was expected to be at version {expectedVersion}, but was found at version {session?.Version ?? 0}.");
    }
    else if (session.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the session entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        session.Version,
        expectedVersion);
    }
    else
    {
      session.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
      await _context.SynchronizeCustomAttributesAsync(KrakenDb.Sessions.Table, session.SessionId, session.GetCustomAttributes(), cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  private async Task<SessionEntity?> FindAsync(DomainEvent @event, CancellationToken cancellationToken)
  {
    return await _context.Sessions.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
  }
}
