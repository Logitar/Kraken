using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms.Events;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Tokens;

namespace Logitar.Kraken.Core.Realms;

public class Realm : AggregateRoot
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

  private JwtSecret? _secret = null;
  public JwtSecret Secret
  {
    get => _secret ?? throw new InvalidOperationException("The realm has not been initialized.");
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
  private bool _requireUniqueEmail = false;
  public bool RequireUniqueEmail
  {
    get => _requireUniqueEmail;
    set
    {
      if (_requireUniqueEmail != value)
      {
        _requireUniqueEmail = value;
        _updatedEvent.RequireUniqueEmail = value;
      }
    }
  }
  private bool _requireConfirmedAccount = false;
  public bool RequireConfirmedAccount
  {
    get => _requireConfirmedAccount;
    set
    {
      if (_requireConfirmedAccount != value)
      {
        _requireConfirmedAccount = value;
        _updatedEvent.RequireConfirmedAccount = value;
      }
    }
  }
  public IUserSettings UserSettings => new UserSettings(UniqueNameSettings, PasswordSettings, RequireUniqueEmail, RequireConfirmedAccount);
  public IRoleSettings RoleSettings => new RoleSettings(UniqueNameSettings);

  public Realm(Slug uniqueSlug, JwtSecret? secret = null, ActorId? actorId = null, RealmId? realmId = null) : base(realmId?.StreamId)
  {
    secret ??= JwtSecret.Generate();
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

    _uniqueNameSettings = @event.UniqueNameSettings;
    _passwordSettings = @event.PasswordSettings;
    _requireUniqueEmail = @event.RequireUniqueEmail;
    _requireConfirmedAccount = @event.RequireConfirmedAccount;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new RealmDeleted(), actorId);
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
  }

  public override string ToString() => $"{DisplayName?.Value ?? UniqueSlug.Value} | {base.ToString()}";
}
