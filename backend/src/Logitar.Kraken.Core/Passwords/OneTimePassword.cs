using Logitar.EventSourcing;
using Logitar.Kraken.Core.Passwords.Events;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Users;

namespace Logitar.Kraken.Core.Passwords;

public class OneTimePassword : AggregateRoot, ICustomizable
{
  private OneTimePasswordUpdated _updated = new();

  private Password? _password = null;

  public new OneTimePasswordId Id => new(base.Id);
  public RealmId? RealmId => Id.RealmId;
  public Guid EntityId => Id.EntityId;

  public UserId? UserId { get; private set; }

  public DateTime? ExpiresOn { get; private set; }
  public int? MaximumAttempts { get; private set; }

  public int AttemptCount { get; private set; }
  public bool HasValidationSucceeded { get; private set; }

  private readonly Dictionary<Identifier, string> _customAttributes = [];
  public IReadOnlyDictionary<Identifier, string> CustomAttributes => _customAttributes.AsReadOnly();

  public OneTimePassword(Password password, DateTime? expiresOn = null, int? maximumAttempts = null, UserId? userId = null, ActorId? actorId = null, OneTimePasswordId? oneTimePasswordId = null)
    : base(oneTimePasswordId?.StreamId)
  {
    if (expiresOn.HasValue && expiresOn.Value.AsUniversalTime() <= DateTime.UtcNow)
    {
      throw new ArgumentOutOfRangeException(nameof(expiresOn), "The expiration date and time must be set in the future.");
    }
    if (maximumAttempts.HasValue && maximumAttempts.Value < 1)
    {
      throw new ArgumentOutOfRangeException(nameof(maximumAttempts), "There should be at least one attempt to validate the One-Time Password (OTP).");
    }

    Raise(new OneTimePasswordCreated(password, expiresOn, maximumAttempts, userId), actorId);
  }
  protected virtual void Handle(OneTimePasswordCreated @event)
  {
    _password = @event.Password;

    UserId = @event.UserId;

    ExpiresOn = @event.ExpiresOn;
    MaximumAttempts = @event.MaximumAttempts;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new OneTimePasswordDeleted(), actorId);
    }
  }

  public bool IsExpired(DateTime? moment = null) => ExpiresOn.HasValue && ExpiresOn.Value.AsUniversalTime() <= (moment?.AsUniversalTime() ?? DateTime.UtcNow);

  public void RemoveCustomAttribute(Identifier key)
  {
    if (_customAttributes.Remove(key))
    {
      _updated.CustomAttributes[key] = null;
    }
  }

  public void SetCustomAttribute(Identifier key, string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      RemoveCustomAttribute(key);
    }
    else
    {
      value = value.Trim();

      if (!_customAttributes.TryGetValue(key, out string? existingValue) || existingValue != value)
      {
        _customAttributes[key] = value;
        _updated.CustomAttributes[key] = value;
      }
    }
  }

  public void Update(ActorId? actorId = null)
  {
    if (_updated.HasChanges)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new();
    }
  }
  protected virtual void Handle(OneTimePasswordUpdated @event)
  {
    foreach (KeyValuePair<Identifier, string?> customAttribute in @event.CustomAttributes)
    {
      if (customAttribute.Value == null)
      {
        _customAttributes.Remove(customAttribute.Key);
      }
      else
      {
        _customAttributes[customAttribute.Key] = customAttribute.Value;
      }
    }
  }

  public void Validate(string password, ActorId? actorId = null)
  {
    if (HasValidationSucceeded)
    {
      throw new OneTimePasswordAlreadyUsedException(this);
    }
    else if (IsExpired())
    {
      throw new OneTimePasswordIsExpiredException(this);
    }
    else if (MaximumAttempts.HasValue && MaximumAttempts.Value <= AttemptCount)
    {
      throw new MaximumAttemptsReachedException(this, AttemptCount);
    }
    else if (_password == null || !_password.IsMatch(password))
    {
      Raise(new OneTimePasswordValidationFailed(), actorId);
      throw new IncorrectOneTimePasswordPasswordException(this, password);
    }

    Raise(new OneTimePasswordValidationSucceeded(), actorId);
  }
  protected virtual void Handle(OneTimePasswordValidationFailed _)
  {
    AttemptCount++;
  }
  protected virtual void Handle(OneTimePasswordValidationSucceeded _)
  {
    AttemptCount++;
    HasValidationSucceeded = true;
  }
}
