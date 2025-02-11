using Logitar.EventSourcing;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Realms;
using Logitar.Kraken.Core.Users.Events;

namespace Logitar.Kraken.Core.Users;

public class User : AggregateRoot, ICustomizable
{
  private UserUpdated _updated = new();

  public new UserId Id => new(base.Id);
  public RealmId? RealmId => Id.RealmId;
  public Guid EntityId => Id.EntityId;

  private UniqueName? _uniqueName = null;
  public UniqueName UniqueName => _uniqueName ?? throw new InvalidOperationException("The user has not been initialized yet.");

  private Password? _password = null;
  public bool HasPassword => _password != null;

  private readonly Dictionary<Identifier, string> _customAttributes = [];
  public IReadOnlyDictionary<Identifier, string> CustomAttributes => _customAttributes.AsReadOnly();

  public User() : base()
  {
  }

  public User(UniqueName uniqueName, Password? password = null, ActorId? actorId = null, UserId? userId = null)
    : base(userId?.StreamId)
  {
    Raise(new UserCreated(uniqueName, password), actorId);
  }
  protected virtual void Handle(UserCreated @event)
  {
    _uniqueName = @event.UniqueName;

    _password = @event.Password;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new UserDeleted(), actorId);
    }
  }

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

  public void SetUniqueName(UniqueName uniqueName, ActorId? actorId = null)
  {
    if (_uniqueName != uniqueName)
    {
      Raise(new UserUniqueNameChanged(uniqueName), actorId);
    }
  }
  protected virtual void Handle(UserUniqueNameChanged @event)
  {
    _uniqueName = @event.UniqueName;
  }

  public void Update(ActorId? actorId = null)
  {
    if (_updated.HasChanges)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new();
    }
  }
  protected virtual void Handle(UserUpdated @event)
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
