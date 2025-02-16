using Logitar.Kraken.Contracts.Settings;

namespace Logitar.Kraken.Contracts.Configurations;

public class ConfigurationModel : AggregateModel
{
  public string Secret { get; set; } = string.Empty; // TODO(fpion): JsonIgnore?

  public UniqueNameSettingsModel UniqueNameSettings { get; set; } = new();
  public PasswordSettingsModel PasswordSettings { get; set; } = new();
}
