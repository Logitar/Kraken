using Logitar.EventSourcing;
using Logitar.Kraken.Core.Passwords.Events;
using Logitar.Kraken.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Kraken.EntityFrameworkCore.Relational.Handlers;

internal class OneTimePasswordEvents : INotificationHandler<OneTimePasswordCreated>,
  INotificationHandler<OneTimePasswordDeleted>,
  INotificationHandler<OneTimePasswordUpdated>,
  INotificationHandler<OneTimePasswordValidationFailed>,
  INotificationHandler<OneTimePasswordValidationSucceeded>
{
  private readonly KrakenContext _context;
  private readonly ILogger<OneTimePasswordEvents> _logger;

  public OneTimePasswordEvents(KrakenContext context, ILogger<OneTimePasswordEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task Handle(OneTimePasswordCreated @event, CancellationToken cancellationToken)
  {
    OneTimePasswordEntity? oneTimePassword = await FindAsync(@event, cancellationToken);
    if (oneTimePassword == null)
    {
      if (@event.UserId.HasValue)
      {
        UserEntity user = await _context.FindUserAsync(@event.UserId.Value, cancellationToken);
        oneTimePassword = new(user, @event);
      }
      else
      {
        RealmEntity? realm = await _context.FindRealmAsync(@event.StreamId, cancellationToken);
        oneTimePassword = new(realm, @event);
      }

      _context.OneTimePasswords.Add(oneTimePassword);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
    else
    {
      _logger.LogWarning("{Event}: the One-Time Password (OTP) entity 'StreamId={Id}' already exists.", @event.GetType().Name, @event.StreamId);
    }
  }

  public async Task Handle(OneTimePasswordDeleted @event, CancellationToken cancellationToken)
  {
    OneTimePasswordEntity? oneTimePassword = await FindAsync(@event, cancellationToken);
    if (oneTimePassword == null)
    {
      _logger.LogWarning("{Event}: the One-Time Password (OTP) entity 'StreamId={StreamId}' is already deleted.", @event.GetType().Name, @event.StreamId);
    }
    else
    {
      _context.OneTimePasswords.Remove(oneTimePassword);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(OneTimePasswordUpdated @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    OneTimePasswordEntity? oneTimePassword = await FindAsync(@event, cancellationToken);
    if (oneTimePassword == null || oneTimePassword.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The One-Time Password (OTP) entity was expected to be at version {expectedVersion}, but was found at version {oneTimePassword?.Version ?? 0}.");
    }
    else if (oneTimePassword.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the One-Time Password (OTP) entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        oneTimePassword.Version,
        expectedVersion);
    }
    else
    {
      oneTimePassword.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
      await _context.SynchronizeCustomAttributesAsync(KrakenDb.OneTimePasswords.Table, oneTimePassword.OneTimePasswordId, oneTimePassword.GetCustomAttributes(), cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(OneTimePasswordValidationFailed @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    OneTimePasswordEntity? oneTimePassword = await FindAsync(@event, cancellationToken);
    if (oneTimePassword == null || oneTimePassword.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The One-Time Password (OTP) entity was expected to be at version {expectedVersion}, but was found at version {oneTimePassword?.Version ?? 0}.");
    }
    else if (oneTimePassword.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the One-Time Password (OTP) entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        oneTimePassword.Version,
        expectedVersion);
    }
    else
    {
      oneTimePassword.Fail(@event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  public async Task Handle(OneTimePasswordValidationSucceeded @event, CancellationToken cancellationToken)
  {
    long expectedVersion = @event.Version - 1;

    OneTimePasswordEntity? oneTimePassword = await FindAsync(@event, cancellationToken);
    if (oneTimePassword == null || oneTimePassword.Version < expectedVersion)
    {
      throw new InvalidOperationException($"The One-Time Password (OTP) entity was expected to be at version {expectedVersion}, but was found at version {oneTimePassword?.Version ?? 0}.");
    }
    else if (oneTimePassword.Version > expectedVersion)
    {
      _logger.LogInformation(
        "{Event}: the One-Time Password (OTP) entity 'StreamId={StreamId}' version '{ActualVersion}' is greater than the expected version '{ExpectedVersion}'.",
        @event.GetType().Name,
        @event.StreamId,
        oneTimePassword.Version,
        expectedVersion);
    }
    else
    {
      oneTimePassword.Succeed(@event);

      await _context.SaveChangesAsync(cancellationToken);

      _logger.LogInformation("Handled {Event} event.", @event.GetType().Name);
    }
  }

  private async Task<OneTimePasswordEntity?> FindAsync(DomainEvent @event, CancellationToken cancellationToken)
  {
    return await _context.OneTimePasswords.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
  }
}
