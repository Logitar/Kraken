using Logitar.EventSourcing;
using Logitar.Kraken.Core.Users.Events;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Handlers;

internal class UserEvents : INotificationHandler<UserAddressChanged>,
  INotificationHandler<UserAuthenticated>,
  INotificationHandler<UserCreated>,
  INotificationHandler<UserDeleted>,
  INotificationHandler<UserDisabled>,
  INotificationHandler<UserEmailChanged>,
  INotificationHandler<UserEnabled>,
  INotificationHandler<UserIdentifierChanged>,
  INotificationHandler<UserIdentifierRemoved>,
  INotificationHandler<UserPasswordChanged>,
  INotificationHandler<UserPasswordRemoved>,
  INotificationHandler<UserPasswordReset>,
  INotificationHandler<UserPasswordUpdated>,
  INotificationHandler<UserPhoneChanged>,
  INotificationHandler<UserRoleAdded>,
  INotificationHandler<UserRoleRemoved>,
  INotificationHandler<UserSignedIn>,
  INotificationHandler<UserUniqueNameChanged>,
  INotificationHandler<UserUpdated>
{
  private readonly KrakenContext _context;
  private readonly ILogger<UserEvents> _logger;

  public UserEvents(KrakenContext context, ILogger<UserEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task Handle(UserAddressChanged @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    UserEntity? user = await FindAsync(@event, cancellationToken);
    if (user == null || user.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The user entity was expected to be at version {expectedVersion}, but was found at version {user?.Version ?? 0}.");
    }
    else if (user.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the user entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        user.Version,
        expectedVersion);
    }
    else
    {
      user.SetAddress(@event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(UserAuthenticated @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    UserEntity? user = await FindAsync(@event, cancellationToken);
    if (user == null || user.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The user entity was expected to be at version {expectedVersion}, but was found at version {user?.Version ?? 0}.");
    }
    else if (user.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the user entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        user.Version,
        expectedVersion);
    }
    else
    {
      user.Authenticate(@event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(UserCreated @event, CancellationToken cancellationToken)
  {
    UserEntity? user = await FindAsync(@event, cancellationToken);
    if (user == null)
    {
      RealmEntity? realm = await _context.FindRealmAsync(@event.StreamId, cancellationToken);

      user = new(realm, @event);

      _context.Users.Add(user);

      await _context.SaveChangesAsync(cancellationToken);
      await UpdateActorAsync(user, cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
    else
    {
      _logger.LogWarning("{Event}: the user entity 'StreamId={Id}' already exists.", @event.GetType().Name, @event.StreamId);
    }
  }

  public async Task Handle(UserDeleted @event, CancellationToken cancellationToken)
  {
    UserEntity? user = await FindAsync(@event, cancellationToken);
    if (user == null)
    {
      _logger.LogWarning("{Event}: the user entity 'StreamId={StreamId}' is already deleted.", @event.GetType().Name, @event.StreamId);
    }
    else
    {
      _context.Users.Remove(user);

      await _context.SaveChangesAsync(cancellationToken);
      await UpdateActorAsync(user, delete: true, cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(UserDisabled @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    UserEntity? user = await FindAsync(@event, cancellationToken);
    if (user == null || user.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The user entity was expected to be at version {expectedVersion}, but was found at version {user?.Version ?? 0}.");
    }
    else if (user.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the user entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        user.Version,
        expectedVersion);
    }
    else
    {
      user.Disable(@event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(UserEmailChanged @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    UserEntity? user = await FindAsync(@event, cancellationToken);
    if (user == null || user.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The user entity was expected to be at version {expectedVersion}, but was found at version {user?.Version ?? 0}.");
    }
    else if (user.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the user entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        user.Version,
        expectedVersion);
    }
    else
    {
      user.SetEmail(@event);

      await _context.SaveChangesAsync(cancellationToken);
      await UpdateActorAsync(user, cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(UserEnabled @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    UserEntity? user = await FindAsync(@event, cancellationToken);
    if (user == null || user.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The user entity was expected to be at version {expectedVersion}, but was found at version {user?.Version ?? 0}.");
    }
    else if (user.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the user entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        user.Version,
        expectedVersion);
    }
    else
    {
      user.Enable(@event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(UserIdentifierChanged @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    UserEntity? user = await FindAsync(@event, cancellationToken);
    if (user == null || user.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The user entity was expected to be at version {expectedVersion}, but was found at version {user?.Version ?? 0}.");
    }
    else if (user.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the user entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        user.Version,
        expectedVersion);
    }
    else
    {
      user.SetCustomIdentifier(@event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(UserIdentifierRemoved @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    UserEntity? user = await FindAsync(@event, cancellationToken);
    if (user == null || user.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The user entity was expected to be at version {expectedVersion}, but was found at version {user?.Version ?? 0}.");
    }
    else if (user.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the user entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        user.Version,
        expectedVersion);
    }
    else
    {
      user.RemoveCustomIdentifier(@event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(UserPasswordChanged @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    UserEntity? user = await FindAsync(@event, cancellationToken);
    if (user == null || user.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The user entity was expected to be at version {expectedVersion}, but was found at version {user?.Version ?? 0}.");
    }
    else if (user.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the user entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        user.Version,
        expectedVersion);
    }
    else
    {
      user.SetPassword(@event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(UserPasswordRemoved @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    UserEntity? user = await FindAsync(@event, cancellationToken);
    if (user == null || user.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The user entity was expected to be at version {expectedVersion}, but was found at version {user?.Version ?? 0}.");
    }
    else if (user.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the user entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        user.Version,
        expectedVersion);
    }
    else
    {
      user.RemovePassword(@event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(UserPasswordReset @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    UserEntity? user = await FindAsync(@event, cancellationToken);
    if (user == null || user.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The user entity was expected to be at version {expectedVersion}, but was found at version {user?.Version ?? 0}.");
    }
    else if (user.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the user entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        user.Version,
        expectedVersion);
    }
    else
    {
      user.SetPassword(@event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(UserPasswordUpdated @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    UserEntity? user = await FindAsync(@event, cancellationToken);
    if (user == null || user.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The user entity was expected to be at version {expectedVersion}, but was found at version {user?.Version ?? 0}.");
    }
    else if (user.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the user entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        user.Version,
        expectedVersion);
    }
    else
    {
      user.SetPassword(@event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(UserPhoneChanged @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    UserEntity? user = await FindAsync(@event, cancellationToken);
    if (user == null || user.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The user entity was expected to be at version {expectedVersion}, but was found at version {user?.Version ?? 0}.");
    }
    else if (user.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the user entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        user.Version,
        expectedVersion);
    }
    else
    {
      user.SetPhone(@event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(UserRoleAdded @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    UserEntity? user = await FindAsync(@event, cancellationToken);
    if (user == null || user.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The user entity was expected to be at version {expectedVersion}, but was found at version {user?.Version ?? 0}.");
    }
    else if (user.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the user entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        user.Version,
        expectedVersion);
    }
    else
    {
      RoleEntity role = await _context.FindRoleAsync(@event.RoleId, cancellationToken);

      user.AddRole(role, @event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(UserRoleRemoved @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    UserEntity? user = await FindAsync(@event, cancellationToken);
    if (user == null || user.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The user entity was expected to be at version {expectedVersion}, but was found at version {user?.Version ?? 0}.");
    }
    else if (user.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the user entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        user.Version,
        expectedVersion);
    }
    else
    {
      user.RemoveRole(@event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(UserSignedIn @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    UserEntity? user = await FindAsync(@event, cancellationToken);
    if (user == null || user.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The user entity was expected to be at version {expectedVersion}, but was found at version {user?.Version ?? 0}.");
    }
    else if (user.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the user entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        user.Version,
        expectedVersion);
    }
    else
    {
      user.SignIn(@event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(UserUniqueNameChanged @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    UserEntity? user = await FindAsync(@event, cancellationToken);
    if (user == null || user.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The user entity was expected to be at version {expectedVersion}, but was found at version {user?.Version ?? 0}.");
    }
    else if (user.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the user entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        user.Version,
        expectedVersion);
    }
    else
    {
      user.SetUniqueName(@event);

      await _context.SaveChangesAsync(cancellationToken);
      await UpdateActorAsync(user, cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(UserUpdated @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    UserEntity? user = await FindAsync(@event, cancellationToken);
    if (user == null || user.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The user entity was expected to be at version {expectedVersion}, but was found at version {user?.Version ?? 0}.");
    }
    else if (user.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the user entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        user.Version,
        expectedVersion);
    }
    else
    {
      user.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
      await UpdateActorAsync(user, cancellationToken);
      await _context.SynchronizeCustomAttributesAsync(KrakenDb.Users.Table, user.UserId, user.GetCustomAttributes(), cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  private async Task<UserEntity?> FindAsync(DomainEvent @event, CancellationToken cancellationToken)
  {
    return await _context.Users
      .Include(x => x.Identifiers)
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
  }

  private async Task UpdateActorAsync(UserEntity user, CancellationToken cancellationToken)
  {
    await UpdateActorAsync(user, delete: false, cancellationToken);
  }
  private async Task UpdateActorAsync(UserEntity user, bool delete, CancellationToken cancellationToken)
  {
    ActorEntity? actor = await _context.Actors.SingleOrDefaultAsync(x => x.Id == user.Id, cancellationToken);
    if (actor == null)
    {
      actor = new(user);

      _context.Actors.Add(actor);
    }
    else
    {
      actor.Update(user);
    }

    if (delete)
    {
      actor.Delete();
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}
