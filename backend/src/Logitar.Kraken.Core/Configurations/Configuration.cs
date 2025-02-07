using Logitar.EventSourcing;
using Logitar.Kraken.Core.Configurations.Events;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Tokens;

namespace Logitar.Kraken.Core.Configurations;

public class Configuration : AggregateRoot
{
  private ConfigurationUpdated _updatedEvent = new();

  public new ConfigurationId Id => new(base.Id);

  private JwtSecret? _secret = null;
  public JwtSecret Secret
  {
    get => _secret ?? throw new InvalidOperationException("The configuration has not been initialized.");
    set
    {
      if (_secret != value)
      {
        _secret = value;
        _updatedEvent.Secret = value;
      }
    }
  }

  private UniqueNameSettings? _uniqueNameSettings = null;
  public UniqueNameSettings UniqueNameSettings
  {
    get => _uniqueNameSettings ?? throw new InvalidOperationException("The configuration has not been initialized.");
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
    get => _passwordSettings ?? throw new InvalidOperationException("The configuration has not been initialized.");
    set
    {
      if (_passwordSettings != value)
      {
        _passwordSettings = value;
        _updatedEvent.PasswordSettings = value;
      }
    }
  }
  public IUserSettings UserSettings => new UserSettings(UniqueNameSettings, PasswordSettings, RequireUniqueEmail: false, RequireConfirmedAccount: false);
  public IRoleSettings RoleSettings => new RoleSettings(UniqueNameSettings);

  private LoggingSettings? _loggingSettings = null;
  public LoggingSettings LoggingSettings
  {
    get => _loggingSettings ?? throw new InvalidOperationException("The configuration has not been initialized.");
    set
    {
      if (_loggingSettings != value)
      {
        _loggingSettings = value;
        _updatedEvent.LoggingSettings = value;
      }
    }
  }

  public Configuration() : base()
  {
  }

  private Configuration(JwtSecret secret, UniqueNameSettings uniqueNameSettings, PasswordSettings passwordSettings, LoggingSettings loggingSettings, ActorId actorId, ConfigurationId configurationId)
    : base(configurationId.StreamId)
  {
    Raise(new ConfigurationInitialized(secret, uniqueNameSettings, passwordSettings, loggingSettings), actorId);
  }
  protected virtual void Handle(ConfigurationInitialized @event)
  {
    _secret = @event.Secret;
    _uniqueNameSettings = @event.UniqueNameSettings;
    _passwordSettings = @event.PasswordSettings;
    _loggingSettings = @event.LoggingSettings;
  }

  public static Configuration Initialize(ActorId actorId)
  {
    JwtSecret secret = JwtSecret.Generate();
    UniqueNameSettings uniqueNameSettings = new();
    PasswordSettings passwordSettings = new();
    LoggingSettings loggingSettings = new();

    return new Configuration(secret, uniqueNameSettings, passwordSettings, loggingSettings, actorId, new ConfigurationId());
  }

  public void Update(ActorId? actorId = null)
  {
    if (_updatedEvent.HasChanges)
    {
      Raise(_updatedEvent, actorId, DateTime.Now);
      _updatedEvent = new();
    }
  }
  protected virtual void Handle(ConfigurationUpdated @event)
  {
    if (@event.Secret != null)
    {
      _secret = @event.Secret;
    }

    if (@event.UniqueNameSettings != null)
    {
      _uniqueNameSettings = @event.UniqueNameSettings;
    }
    if (@event.PasswordSettings != null)
    {
      _passwordSettings = @event.PasswordSettings;
    }

    if (@event.LoggingSettings != null)
    {
      _loggingSettings = @event.LoggingSettings;
    }
  }
}
