using Logitar.Kraken.Contracts.Senders;

namespace Logitar.Kraken.Core.Senders;

public abstract record SenderSettings
{
  [JsonIgnore]
  public abstract SenderProvider Provider { get; }
}
