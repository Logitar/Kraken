using Logitar.Kraken.Contracts.Settings;

namespace Logitar.Kraken.Contracts.Configurations;

public record ReplaceConfigurationPayload
{
  public string? Secret { get; set; } // TODO(fpion): how to "generate" a new secret?

  public UniqueNameSettingsModel UniqueNameSettings { get; set; } = new();
  public PasswordSettingsModel PasswordSettings { get; set; } = new();
}
