using Logitar.Kraken.Contracts.Logging;

namespace Logitar.Kraken.Contracts.Settings;

public interface ILoggingSettings
{
  LoggingExtent Extent { get; }
  bool OnlyErrors { get; }
}
