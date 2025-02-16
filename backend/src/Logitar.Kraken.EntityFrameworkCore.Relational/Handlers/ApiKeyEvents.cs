using Logitar.EventSourcing;
using Logitar.Kraken.Core.ApiKeys.Events;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Handlers;

internal class ApiKeyEvents : INotificationHandler<ApiKeyAuthenticated>,
  INotificationHandler<ApiKeyCreated>,
  INotificationHandler<ApiKeyDeleted>,
  INotificationHandler<ApiKeyRoleAdded>,
  INotificationHandler<ApiKeyRoleRemoved>,
  INotificationHandler<ApiKeyUpdated>
{
  private readonly KrakenContext _context;
  private readonly ILogger<ApiKeyEvents> _logger;

  public ApiKeyEvents(KrakenContext context, ILogger<ApiKeyEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task Handle(ApiKeyAuthenticated @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    ApiKeyEntity? apiKey = await FindAsync(@event, cancellationToken);
    if (apiKey == null || apiKey.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The API key entity was expected to be at version {expectedVersion}, but was found at version {apiKey?.Version ?? 0}.");
    }
    else if (apiKey.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the API key entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        apiKey.Version,
        expectedVersion);
    }
    else
    {
      apiKey.Authenticate(@event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(ApiKeyCreated @event, CancellationToken cancellationToken)
  {
    ApiKeyEntity? apiKey = await FindAsync(@event, cancellationToken);
    if (apiKey == null)
    {
      RealmEntity? realm = await _context.FindRealmAsync(@event.StreamId, cancellationToken);

      apiKey = new(realm, @event);

      _context.ApiKeys.Add(apiKey);

      await _context.SaveChangesAsync(cancellationToken);
      await UpdateActorAsync(apiKey, cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
    else
    {
      _logger.LogWarning("{Event}: the API key entity 'StreamId={Id}' already exists.", @event.GetType().Name, @event.StreamId);
    }
  }

  public async Task Handle(ApiKeyDeleted @event, CancellationToken cancellationToken)
  {
    ApiKeyEntity? apiKey = await FindAsync(@event, cancellationToken);
    if (apiKey == null)
    {
      _logger.LogWarning("{Event}: the API key entity 'StreamId={StreamId}' is already deleted.", @event.GetType().Name, @event.StreamId);
    }
    else
    {
      _context.ApiKeys.Remove(apiKey);

      await _context.SaveChangesAsync(cancellationToken);
      await UpdateActorAsync(apiKey, delete: true, cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(ApiKeyRoleAdded @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    ApiKeyEntity? apiKey = await FindAsync(@event, cancellationToken);
    if (apiKey == null || apiKey.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The API key entity was expected to be at version {expectedVersion}, but was found at version {apiKey?.Version ?? 0}.");
    }
    else if (apiKey.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the API key entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        apiKey.Version,
        expectedVersion);
    }
    else
    {
      RoleEntity role = await _context.FindRoleAsync(@event.RoleId, cancellationToken);

      apiKey.AddRole(role, @event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(ApiKeyRoleRemoved @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    ApiKeyEntity? apiKey = await FindAsync(@event, cancellationToken);
    if (apiKey == null || apiKey.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The API key entity was expected to be at version {expectedVersion}, but was found at version {apiKey?.Version ?? 0}.");
    }
    else if (apiKey.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the API key entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        apiKey.Version,
        expectedVersion);
    }
    else
    {
      apiKey.RemoveRole(@event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(ApiKeyUpdated @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    ApiKeyEntity? apiKey = await FindAsync(@event, cancellationToken);
    if (apiKey == null || apiKey.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The API key entity was expected to be at version {expectedVersion}, but was found at version {apiKey?.Version ?? 0}.");
    }
    else if (apiKey.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the API key entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        apiKey.Version,
        expectedVersion);
    }
    else
    {
      apiKey.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
      await UpdateActorAsync(apiKey, cancellationToken);
      await _context.SynchronizeCustomAttributesAsync(KrakenDb.ApiKeys.Table, apiKey.ApiKeyId, apiKey.GetCustomAttributes(), cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  private async Task<ApiKeyEntity?> FindAsync(DomainEvent @event, CancellationToken cancellationToken)
  {
    return await _context.ApiKeys.Include(x => x.Roles).SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
  }

  private async Task UpdateActorAsync(ApiKeyEntity apiKey, CancellationToken cancellationToken)
  {
    await UpdateActorAsync(apiKey, delete: false, cancellationToken);
  }
  private async Task UpdateActorAsync(ApiKeyEntity apiKey, bool delete, CancellationToken cancellationToken)
  {
    ActorEntity? actor = await _context.Actors.SingleOrDefaultAsync(x => x.Id == apiKey.Id, cancellationToken);
    if (actor == null)
    {
      actor = new(apiKey);

      _context.Actors.Add(actor);
    }
    else
    {
      actor.Update(apiKey);
    }

    if (delete)
    {
      actor.Delete();
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}
