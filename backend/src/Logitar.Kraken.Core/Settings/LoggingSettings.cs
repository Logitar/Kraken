using FluentValidation;
using Logitar.Kraken.Contracts.Logging;
using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Core.Settings.Validators;

namespace Logitar.Kraken.Core.Settings;

public record LoggingSettings : ILoggingSettings
{
  public LoggingExtent Extent { get; }
  public bool OnlyErrors { get; }

  public LoggingSettings() : this(LoggingExtent.ActivityOnly)
  {
  }

  public LoggingSettings(ILoggingSettings logging) : this(logging.Extent, logging.OnlyErrors)
  {
  }

  [JsonConstructor]
  public LoggingSettings(LoggingExtent extent, bool onlyErrors = false)
  {
    Extent = extent;
    OnlyErrors = onlyErrors;
    new LoggingSettingsValidator().ValidateAndThrow(this);
  }
}
