using FluentValidation;
using FluentValidation.Results;
using Logitar.EventSourcing;
using Logitar.Kraken.Core.ApiKeys.Events;
using Logitar.Kraken.Core.Passwords;
using Logitar.Kraken.Core.Roles;

namespace Logitar.Kraken.Core.ApiKeys;

public class ApiKey : AggregateRoot, ICustomizable
{
  private Password? _secret = null;
  private ApiKeyUpdated _updatedEvent = new();

  public new ApiKeyId Id => new(base.Id);

  private DisplayName? _name = null;
  public DisplayName Name
  {
    get => _name ?? throw new InvalidOperationException("The API key has not been initialized.");
    set
    {
      if (_name != value)
      {
        _name = value;
        _updatedEvent.Name = value;
      }
    }
  }
  private Description? _description = null;
  public Description? Description
  {
    get => _description;
    set
    {
      if (_description != value)
      {
        _description = value;
        _updatedEvent.Description = new Change<Description>(value);
      }
    }
  }
  private DateTime? _expiresOn = null;
  public DateTime? ExpiresOn
  {
    get => _expiresOn;
    set
    {
      if (_expiresOn != value)
      {
        if (!value.HasValue || (_expiresOn.HasValue && _expiresOn.Value.AsUniversalTime() < value.Value.AsUniversalTime()))
        {
          ValidationFailure failure = new(nameof(ExpiresOn), "The API key expiration cannot be extended, nor removed.", value)
          {
            ErrorCode = "ExpirationValidator"
          };
          throw new ValidationException([failure]);
        }
        else if (value.Value.AsUniversalTime() < DateTime.UtcNow)
        {
          throw new ArgumentOutOfRangeException(nameof(ExpiresOn), "The value must be a date and time set in the future.");
        }

        _expiresOn = value;
        _updatedEvent.ExpiresOn = value;
      }
    }
  }

  public DateTime? AuthenticatedOn { get; private set; }

  private readonly Dictionary<Identifier, string> _customAttributes = [];
  public IReadOnlyDictionary<Identifier, string> CustomAttributes => _customAttributes.AsReadOnly();

  private readonly HashSet<RoleId> _roles = [];
  public IReadOnlyCollection<RoleId> Roles => _roles.ToList().AsReadOnly();

  public ApiKey(Password secret, DisplayName name, ActorId? actorId = null, ApiKeyId? apiKeyId = null) : base(apiKeyId?.StreamId)
  {
    Raise(new ApiKeyCreated(secret, name), actorId);
  }
  protected virtual void Handle(ApiKeyCreated @event)
  {
    _secret = @event.Secret;

    _name = @event.Name;
  }

  public void AddRole(Role role, ActorId? actorId = null)
  {
    if (Id.RealmId != role.Id.RealmId)
    {
      throw new NotImplementedException(); // TODO(fpion): implement
    }

    if (!HasRole(role))
    {
      Raise(new ApiKeyRoleAdded(role.Id), actorId);
    }
  }
  protected virtual void Handle(ApiKeyRoleAdded @event)
  {
    _roles.Add(@event.RoleId);
  }

  public void Authenticate(string secret, ActorId? actorId = null)
  {
    if (IsExpired())
    {
      throw new ApiKeyIsExpiredException(this);
    }
    else if (_secret == null || !_secret.IsMatch(secret))
    {
      throw new IncorrectApiKeySecretException(this, secret);
    }

    actorId ??= new(Id.Value);
    Raise(new ApiKeyAuthenticated(), actorId.Value);
  }
  protected virtual void Handle(ApiKeyAuthenticated @event)
  {
    AuthenticatedOn = @event.OccurredOn;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new ApiKeyDeleted(), actorId);
    }
  }

  public bool HasRole(Role role) => HasRole(role.Id);
  public bool HasRole(RoleId roleId) => _roles.Contains(roleId);

  public bool IsExpired(DateTime? moment = null) => ExpiresOn.HasValue && ExpiresOn.Value.AsUniversalTime() <= (moment?.AsUniversalTime() ?? DateTime.UtcNow);

  public void RemoveCustomAttribute(Identifier key)
  {
    if (_customAttributes.Remove(key))
    {
      _updatedEvent.CustomAttributes[key] = null;
    }
  }

  public void RemoveRole(Role role, ActorId? actorId = null)
  {
    if (HasRole(role))
    {
      Raise(new ApiKeyRoleRemoved(role.Id), actorId);
    }
  }
  public void RemoveRole(RoleId roleId, ActorId? actorId = null)
  {
    if (HasRole(roleId))
    {
      Raise(new ApiKeyRoleRemoved(roleId), actorId);
    }
  }
  protected virtual void Handle(ApiKeyRoleRemoved @event)
  {
    _roles.Remove(@event.RoleId);
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

  public void Update(ActorId? actorId = null)
  {
    if (_updatedEvent.HasChanges)
    {
      Raise(_updatedEvent, actorId, DateTime.Now);
      _updatedEvent = new();
    }
  }
  protected virtual void Handle(ApiKeyUpdated @event)
  {
    if (@event.Name != null)
    {
      _name = @event.Name;
    }
    if (@event.Description != null)
    {
      _description = @event.Description.Value;
    }
    if (@event.ExpiresOn.HasValue)
    {
      _expiresOn = @event.ExpiresOn.Value;
    }

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

  public override string ToString() => $"{Name} | {base.ToString()}";
}
