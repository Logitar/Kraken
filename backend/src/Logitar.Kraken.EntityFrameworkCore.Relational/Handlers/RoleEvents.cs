using Logitar.EventSourcing;
using Logitar.Kraken.Core.Roles.Events;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Handlers;

internal class RoleEvents : INotificationHandler<RoleCreated>,
  INotificationHandler<RoleDeleted>,
  INotificationHandler<RoleUniqueNameChanged>,
  INotificationHandler<RoleUpdated>
{
  private readonly KrakenContext _context;
  private readonly ILogger<RoleEvents> _logger;

  public RoleEvents(KrakenContext context, ILogger<RoleEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task Handle(RoleCreated @event, CancellationToken cancellationToken)
  {
    RoleEntity? role = await FindAsync(@event, cancellationToken);
    if (role == null)
    {
      RealmEntity? realm = await _context.FindRealmAsync(@event.StreamId, cancellationToken);

      role = new(realm, @event);

      _context.Roles.Add(role);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
    else
    {
      _logger.LogWarning("{Event}: the role entity 'StreamId={Id}' already exists.", @event.GetType().Name, @event.StreamId);
    }
  }

  public async Task Handle(RoleDeleted @event, CancellationToken cancellationToken)
  {
    RoleEntity? role = await FindAsync(@event, cancellationToken);
    if (role == null)
    {
      _logger.LogWarning("{Event}: the role entity 'StreamId={StreamId}' is already deleted.", @event.GetType().Name, @event.StreamId);
    }
    else
    {
      _context.Roles.Remove(role);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(RoleUniqueNameChanged @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    RoleEntity? role = await FindAsync(@event, cancellationToken);
    if (role == null || role.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The role entity was expected to be at version {expectedVersion}, but was found at version {role?.Version ?? 0}.");
    }
    else if (role.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the role entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        role.Version,
        expectedVersion);
    }
    else
    {
      role.SetUniqueName(@event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(RoleUpdated @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    RoleEntity? role = await FindAsync(@event, cancellationToken);
    if (role == null || role.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The role entity was expected to be at version {expectedVersion}, but was found at version {role?.Version ?? 0}.");
    }
    else if (role.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the role entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        role.Version,
        expectedVersion);
    }
    else
    {
      role.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
      await _context.SynchronizeCustomAttributesAsync(KrakenDb.Roles.Table, role.RoleId, role.GetCustomAttributes(), cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  private async Task<RoleEntity?> FindAsync(DomainEvent @event, CancellationToken cancellationToken)
  {
    return await _context.Roles.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
  }
}
