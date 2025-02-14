using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms.Events;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Handlers;

internal class RealmEvents : INotificationHandler<RealmCreated>,
  INotificationHandler<RealmDeleted>,
  INotificationHandler<RealmUniqueSlugChanged>,
  INotificationHandler<RealmUpdated>
{
  private readonly KrakenContext _context;
  private readonly ILogger<RealmEvents> _logger;

  public RealmEvents(KrakenContext context, ILogger<RealmEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task Handle(RealmCreated @event, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await FindAsync(@event, cancellationToken);
    if (realm == null)
    {
      realm = new(@event);

      _context.Realms.Add(realm);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
    else
    {
      _logger.LogWarning("{Event}: the realm entity 'StreamId={Id}' already exists.", @event.GetType().Name, @event.StreamId);
    }
  }

  public async Task Handle(RealmDeleted @event, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await FindAsync(@event, cancellationToken);
    if (realm == null)
    {
      _logger.LogWarning("{Event}: the realm entity 'StreamId={StreamId}' is already deleted.", @event.GetType().Name, @event.StreamId);
    }
    else
    {
      _context.Realms.Remove(realm);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(RealmUniqueSlugChanged @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    RealmEntity? realm = await FindAsync(@event, cancellationToken);
    if (realm == null || realm.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The realm entity was expected to be at version {expectedVersion}, but was found at version {realm?.Version ?? 0}.");
    }
    else if (realm.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the realm entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        realm.Version,
        expectedVersion);
    }
    else
    {
      realm.SetUniqueSlug(@event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(RealmUpdated @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    RealmEntity? realm = await FindAsync(@event, cancellationToken);
    if (realm == null || realm.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The realm entity was expected to be at version {expectedVersion}, but was found at version {realm?.Version ?? 0}.");
    }
    else if (realm.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the realm entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        realm.Version,
        expectedVersion);
    }
    else
    {
      realm.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  private async Task<RealmEntity?> FindAsync(DomainEvent @event, CancellationToken cancellationToken)
  {
    return await _context.Realms.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
  }
}
