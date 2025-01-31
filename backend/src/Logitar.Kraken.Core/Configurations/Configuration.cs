using Logitar.EventSourcing;
using Logitar.Kraken.Core.Configurations.Events;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Tokens;

namespace Logitar.Kraken.Core.Configurations;

public class Configuration : AggregateRoot
{
  public new ConfigurationId Id => new(base.Id);

  private JwtSecret? _secret = null;
  public JwtSecret Secret => _secret ?? throw new InvalidOperationException("The configuration has not been initialized.");

  private UniqueNameSettings? _uniqueNameSettings = null;
  public UniqueNameSettings UniqueNameSettings => _uniqueNameSettings ?? throw new InvalidOperationException("The configuration has not been initialized.");
  private PasswordSettings? _passwordSettings = null;
  public PasswordSettings PasswordSettings => _passwordSettings ?? throw new InvalidOperationException("The configuration has not been initialized.");
  public IUserSettings UserSettings => new UserSettings(UniqueNameSettings, PasswordSettings, RequireUniqueEmail: false, RequireConfirmedAccount: false);
  public IRoleSettings RoleSettings => new RoleSettings(UniqueNameSettings);

  private LoggingSettings? _loggingSettings = null;
  public LoggingSettings LoggingSettings => _loggingSettings ?? throw new InvalidOperationException("The configuration has not been initialized.");

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
}
