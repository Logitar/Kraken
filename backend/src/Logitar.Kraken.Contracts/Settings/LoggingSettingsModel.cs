using Logitar.Kraken.Contracts.Logging;

namespace Logitar.Kraken.Contracts.Settings;

public record LoggingSettingsModel : ILoggingSettings
{
  public LoggingExtent Extent { get; set; }
  public bool OnlyErrors { get; set; }
}
