using Logitar.EventSourcing;
using Logitar.Kraken.Core.Configurations.Events;
using Logitar.Kraken.Core.Settings;

namespace Logitar.Kraken.Core.Configurations;

public class Configuration : AggregateRoot
{
  public new ConfigurationId Id { get; } = new();

  // TODO(fpion): Secret

  private UniqueNameSettings? _uniqueNameSettings = null;
  public UniqueNameSettings UniqueNameSettings => _uniqueNameSettings ?? throw new InvalidOperationException("The configuration has not been initialized.");
  private PasswordSettings? _passwordSettings = null;
  public PasswordSettings PasswordSettings => _passwordSettings ?? throw new InvalidOperationException("The configuration has not been initialized.");

  public Configuration() : base()
  {
  }

  private Configuration(UniqueNameSettings uniqueNameSettings, PasswordSettings passwordSettings, ActorId? actorId, ConfigurationId configurationId)
    : base(configurationId.StreamId)
  {
    Raise(new ConfigurationInitialized(uniqueNameSettings, passwordSettings), actorId);
  }
  protected virtual void Handle(ConfigurationInitialized @event)
  {
    _uniqueNameSettings = @event.UniqueNameSettings;
    _passwordSettings = @event.PasswordSettings;
  }

  public static Configuration Initialize(ActorId? actorId = null)
  {
    UniqueNameSettings uniqueNameSettings = new();
    PasswordSettings passwordSettings = new();
    ConfigurationId configurationId = new();
    return new Configuration(uniqueNameSettings, passwordSettings, actorId, configurationId);
  }
}
