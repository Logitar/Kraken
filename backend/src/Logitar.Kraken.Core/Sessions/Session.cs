using Logitar.EventSourcing;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Sessions.Events;
using Logitar.Kraken.Core.Users;

namespace Logitar.Kraken.Core.Sessions;

public class Session : AggregateRoot, ICustomizable
{
  private SessionUpdated _updatedEvent = new();

  public new SessionId Id => new(base.Id);
  public RealmId? RealmId => Id.RealmId;
  public Guid EntityId => Id.EntityId;

  private UserId? _userId = null;
  public UserId UserId => _userId ?? throw new InvalidOperationException("The session has not been initialized.");

  private Password? _secret = null;
  public bool IsPersistent => _secret != null;

  public bool IsActive { get; private set; }

  private readonly Dictionary<Identifier, string> _customAttributes = [];
  public IReadOnlyDictionary<Identifier, string> CustomAttributes => _customAttributes.AsReadOnly();

  public Session(User user, Password? secret = null, ActorId? actorId = null, SessionId? id = null) : base(id?.StreamId)
  {
    if (RealmId != user.RealmId)
    {
      throw new InvalidRealmException(RealmId, user.RealmId);
    }

    actorId ??= new(user.Id.Value);
    Raise(new SessionCreated(user.Id, secret), actorId.Value);
  }
  protected virtual void Handle(SessionCreated @event)
  {
    _userId = @event.UserId;

    _secret = @event.Secret;

    IsActive = true;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new SessionDeleted(), actorId);
    }
  }

  public void RemoveCustomAttribute(Identifier key)
  {
    if (_customAttributes.Remove(key))
    {
      _updatedEvent.CustomAttributes[key] = null;
    }
  }

  public void Renew(string currentSecret, Password newSecret, ActorId? actorId = default)
  {
    if (!IsActive)
    {
      throw new SessionIsNotActiveException(this);
    }
    else if (_secret == null)
    {
      throw new SessionIsNotPersistentException(this);
    }
    else if (!_secret.IsMatch(currentSecret))
    {
      throw new IncorrectSessionSecretException(this, currentSecret);
    }

    actorId ??= new(UserId.Value);
    Raise(new SessionRenewed(newSecret), actorId.Value);
  }
  protected virtual void Handle(SessionRenewed @event)
  {
    _secret = @event.Secret;
  }

  public void SetCustomAttribute(Identifier key, string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      RemoveCustomAttribute(key);
    }
    value = value.Trim();

    if (!_customAttributes.TryGetValue(key, out string? existingValue) || existingValue != value)
    {
      _customAttributes[key] = value;
      _updatedEvent.CustomAttributes[key] = value;
    }
  }

  public void SignOut(ActorId? actorId = default)
  {
    if (IsActive)
    {
      actorId ??= new(UserId.Value);
      Raise(new SessionSignedOut(), actorId.Value);
    }
  }
  protected virtual void Handle(SessionSignedOut _)
  {
    IsActive = false;
  }

  public void Update(ActorId? actorId = null)
  {
    if (_updatedEvent.HasChanges)
    {
      Raise(_updatedEvent, actorId, DateTime.Now);
      _updatedEvent = new();
    }
  }
  protected virtual void Handle(SessionUpdated @event)
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
}
