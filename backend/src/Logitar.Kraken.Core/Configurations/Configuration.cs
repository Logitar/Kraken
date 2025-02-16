using Logitar.EventSourcing;
using Logitar.Kraken.Core.Configurations.Events;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Tokens;

namespace Logitar.Kraken.Core.Configurations;

public class Configuration : AggregateRoot
{
  private ConfigurationUpdated _updatedEvent = new();

  public new ConfigurationId Id { get; } = new();

  private Secret? _secret = null;
  public Secret Secret
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

  public Configuration() : base()
  {
  }

  private Configuration(Secret secret, UniqueNameSettings uniqueNameSettings, PasswordSettings passwordSettings, ActorId? actorId, ConfigurationId configurationId)
    : base(configurationId.StreamId)
  {
    Raise(new ConfigurationInitialized(secret, uniqueNameSettings, passwordSettings), actorId);
  }
  protected virtual void Handle(ConfigurationInitialized @event)
  {
    _secret = @event.Secret;

    _uniqueNameSettings = @event.UniqueNameSettings;
    _passwordSettings = @event.PasswordSettings;
  }

  public static Configuration Initialize(Secret secret, ActorId? actorId = null)
  {
    UniqueNameSettings uniqueNameSettings = new();
    PasswordSettings passwordSettings = new();
    ConfigurationId configurationId = new();
    return new Configuration(secret, uniqueNameSettings, passwordSettings, actorId, configurationId);
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
  }
}
