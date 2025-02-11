using Logitar.Kraken.Contracts.Settings;

namespace Logitar.Kraken.Contracts.Configurations;

public record UpdateConfigurationPayload
{
  public string? Secret { get; set; } // TODO(fpion): how to "generate" a new secret?

  public UniqueNameSettingsModel? UniqueNameSettings { get; set; }
  public PasswordSettingsModel? PasswordSettings { get; set; }
}
