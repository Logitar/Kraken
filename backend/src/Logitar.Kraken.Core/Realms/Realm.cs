using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms.Events;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Tokens;

namespace Logitar.Kraken.Core.Realms;

public class Realm : AggregateRoot, ICustomizable
{
  private RealmUpdated _updatedEvent = new();

  public new RealmId Id => new(base.Id);

  private Slug? _uniqueSlug = null;
  public Slug UniqueSlug => _uniqueSlug ?? throw new InvalidOperationException("The realm has not been initialized.");
  private DisplayName? _displayName = null;
  public DisplayName? DisplayName
  {
    get => _displayName;
    set
    {
      if (_displayName != value)
      {
        _displayName = value;
        _updatedEvent.DisplayName = new Change<DisplayName>(value);
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

  private Secret? _secret = null;
  public Secret? Secret
  {
    get => _secret;
    set
    {
      if (_secret != value)
      {
        _secret = value;
        _updatedEvent.Secret = value;
      }
    }
  }
  private Url? _url = null;
  public Url? Url
  {
    get => _url;
    set
    {
      if (_url != value)
      {
        _url = value;
        _updatedEvent.Url = new Change<Url>(value);
      }
    }
  }

  private UniqueNameSettings? _uniqueNameSettings = null;
  public UniqueNameSettings UniqueNameSettings
  {
    get => _uniqueNameSettings ?? throw new InvalidOperationException("The realm has not been initialized.");
    set
    {
      if (_uniqueNameSettings != value)
      {
        _uniqueNameSettings = value;
        _updatedEvent.UniqueNameSettings = value;
      }
    }
  }
  private PasswordSettings? _passwordSettings = null;
  public PasswordSettings PasswordSettings
  {
    get => _passwordSettings ?? throw new InvalidOperationException("The realm has not been initialized.");
    set
    {
      if (_passwordSettings != value)
      {
        _passwordSettings = value;
        _updatedEvent.PasswordSettings = value;
      }
    }
  }
  private bool? _requireUniqueEmail = null;
  public bool RequireUniqueEmail
  {
    get => _requireUniqueEmail ?? throw new InvalidOperationException("The realm has not been initialized.");
    set
    {
      if (_requireUniqueEmail != value)
      {
        _requireUniqueEmail = value;
        _updatedEvent.RequireUniqueEmail = value;
      }
    }
  }
  private bool? _requireConfirmedAccount = null;
  public bool RequireConfirmedAccount
  {
    get => _requireConfirmedAccount ?? throw new InvalidOperationException("The realm has not been initialized.");
    set
    {
      if (_requireConfirmedAccount != value)
      {
        _requireConfirmedAccount = value;
        _updatedEvent.RequireConfirmedAccount = value;
      }
    }
  }

  private readonly Dictionary<Identifier, string> _customAttributes = [];
  public IReadOnlyDictionary<Identifier, string> CustomAttributes => _customAttributes.AsReadOnly();

  public Realm() : base()
  {
  }

  public Realm(Slug uniqueSlug, Secret secret, ActorId? actorId = null, RealmId? realmId = null)
    : base(realmId?.StreamId)
  {
    UniqueNameSettings uniqueNameSettings = new();
    PasswordSettings passwordSettings = new();
    bool requireUniqueEmail = true;
    bool requireConfirmedAccount = true;
    Raise(new RealmCreated(uniqueSlug, secret, uniqueNameSettings, passwordSettings, requireUniqueEmail, requireConfirmedAccount), actorId);
  }
  protected virtual void Handle(RealmCreated @event)
  {
    _uniqueSlug = @event.UniqueSlug;

    _secret = @event.Secret;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new RealmDeleted(), actorId);
    }
  }

  public void RemoveCustomAttribute(Identifier key)
  {
    if (_customAttributes.Remove(key))
    {
      _updatedEvent.CustomAttributes[key] = null;
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
        _updatedEvent.CustomAttributes[key] = value;
      }
    }
  }

  public void SetUniqueSlug(Slug uniqueSlug, ActorId? actorId = null)
  {
    if (_uniqueSlug != uniqueSlug)
    {
      Raise(new RealmUniqueSlugChanged(uniqueSlug), actorId);
    }
  }
  protected virtual void Handle(RealmUniqueSlugChanged @event)
  {
    _uniqueSlug = @event.UniqueSlug;
  }

  public void Update(ActorId? actorId = null)
  {
    if (_updatedEvent.HasChanges)
    {
      Raise(_updatedEvent, actorId, DateTime.Now);
      _updatedEvent = new();
    }
  }
  protected virtual void Handle(RealmUpdated @event)
  {
    if (@event.DisplayName != null)
    {
      _displayName = @event.DisplayName.Value;
    }
    if (@event.Description != null)
    {
      _description = @event.Description.Value;
    }

    if (@event.Secret != null)
    {
      _secret = @event.Secret;
    }
    if (@event.Url != null)
    {
      _url = @event.Url.Value;
    }

    if (@event.UniqueNameSettings != null)
    {
      _uniqueNameSettings = @event.UniqueNameSettings;
    }
    if (@event.PasswordSettings != null)
    {
      _passwordSettings = @event.PasswordSettings;
    }
    if (@event.RequireUniqueEmail.HasValue)
    {
      _requireUniqueEmail = @event.RequireUniqueEmail.Value;
    }
    if (@event.RequireConfirmedAccount.HasValue)
    {
      _requireConfirmedAccount = @event.RequireConfirmedAccount.Value;
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

  public override string ToString() => $"{DisplayName?.Value ?? UniqueSlug.Value} | {base.ToString()}";
}
