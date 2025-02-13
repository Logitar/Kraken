using Logitar.EventSourcing;
using Logitar.Kraken.Core.Configurations.Events;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Tokens;
using Logitar.Security.Cryptography;

namespace Logitar.Kraken.Core.Configurations;

[Trait(Traits.Category, Categories.Unit)]
public class ConfigurationTests
{
  private readonly Secret _secret = new(RandomStringGenerator.GetString());
  private readonly Configuration _configuration;

  public ConfigurationTests()
  {
    _configuration = Configuration.Initialize(_secret);
  }

  [Fact(DisplayName = "Initialize: it should initialize the correct configuration.")]
  public void Given_Arguments_When_Initialize_Then_Initialized()
  {
    Secret secret = new(RandomStringGenerator.GetString());
    ActorId actorId = ActorId.NewId();

    Configuration configuration = Configuration.Initialize(secret, actorId);

    Assert.Equal(actorId, configuration.CreatedBy);
    Assert.Equal(actorId, configuration.UpdatedBy);
    Assert.Same(secret, configuration.Secret);
    Assert.Equal(new UniqueNameSettings(), configuration.UniqueNameSettings);
    Assert.Equal(new PasswordSettings(), configuration.PasswordSettings);
  }

  [Fact(DisplayName = "PasswordSettings: it should handle the updates correctly.")]
  public void Given_PasswordSettingsUpdates_When_setPasswordSettings_Then_UpdatesHandledCorrectly()
  {
    _configuration.ClearChanges();

    _configuration.PasswordSettings = _configuration.PasswordSettings;
    _configuration.Update();
    Assert.False(_configuration.HasChanges);
    Assert.Empty(_configuration.Changes);

    _configuration.PasswordSettings = new PasswordSettings(requiredLength: 6, requiredUniqueChars: 1, requireNonAlphanumeric: false, requireLowercase: true, requireUppercase: true, requireDigit: true, hashingStrategy: "PBKDF2");
    _configuration.Update();
    Assert.True(_configuration.HasChanges);
    Assert.Contains(_configuration.Changes, change => change is ConfigurationUpdated updated && updated.PasswordSettings != null && updated.PasswordSettings.Equals(_configuration.PasswordSettings));
  }

  [Fact(DisplayName = "Secret: it should handle the updates correctly.")]
  public void Given_SecretUpdates_When_setSecret_Then_UpdatesHandledCorrectly()
  {
    _configuration.ClearChanges();

    _configuration.Secret = _configuration.Secret;
    _configuration.Update();
    Assert.False(_configuration.HasChanges);
    Assert.Empty(_configuration.Changes);

    _configuration.Secret = new Secret(RandomStringGenerator.GetString());
    _configuration.Update();
    Assert.True(_configuration.HasChanges);
    Assert.Contains(_configuration.Changes, change => change is ConfigurationUpdated updated && updated.Secret != null && updated.Secret.Equals(_configuration.Secret));
  }

  [Fact(DisplayName = "UniqueNameSettings: it should handle the updates correctly.")]
  public void Given_UniqueNameSettingsUpdates_When_setUniqueNameSettings_Then_UpdatesHandledCorrectly()
  {
    _configuration.ClearChanges();

    _configuration.UniqueNameSettings = _configuration.UniqueNameSettings;
    _configuration.Update();
    Assert.False(_configuration.HasChanges);
    Assert.Empty(_configuration.Changes);

    _configuration.UniqueNameSettings = new UniqueNameSettings(allowedCharacters: null);
    _configuration.Update();
    Assert.True(_configuration.HasChanges);
    Assert.Contains(_configuration.Changes, change => change is ConfigurationUpdated updated && updated.UniqueNameSettings != null && updated.UniqueNameSettings.Equals(_configuration.UniqueNameSettings));
  }

  [Theory(DisplayName = "Update: it should update the configuration.")]
  [InlineData(null)]
  [InlineData("SYSTEM")]
  public void Given_Updates_When_Update_Then_ConfigurationUpdated(string? actorIdValue)
  {
    ActorId? actorId = actorIdValue == null ? null : new(actorIdValue);

    _configuration.ClearChanges();
    _configuration.Update();
    Assert.False(_configuration.HasChanges);
    Assert.Empty(_configuration.Changes);

    _configuration.Secret = new Secret(RandomStringGenerator.GetString());
    _configuration.Update(actorId);
    Assert.Contains(_configuration.Changes, change => change is ConfigurationUpdated updated && updated.ActorId == actorId && (updated.OccurredOn - DateTime.Now) < TimeSpan.FromSeconds(1));
  }
}
