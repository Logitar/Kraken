using Logitar.Kraken.Contracts.Logging;

namespace Logitar.Kraken.Contracts.Settings;

public record LoggingSettingsModel : ILoggingSettings
{
  public LoggingExtent Extent { get; set; } = LoggingExtent.ActivityOnly;
  public bool OnlyErrors { get; set; }

  public LoggingSettingsModel()
  {
  }

  public LoggingSettingsModel(LoggingExtent extent, bool onlyErrors = false)
  {
    Extent = extent;
    OnlyErrors = onlyErrors;
  }

  public LoggingSettingsModel(ILoggingSettings logging) : this(logging.Extent, logging.OnlyErrors)
  {
  }
}
