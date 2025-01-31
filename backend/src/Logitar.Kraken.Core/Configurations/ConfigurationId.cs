using Logitar.EventSourcing;

namespace Logitar.Kraken.Core.Configurations;

public readonly struct ConfigurationId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public ConfigurationId()
  {
    StreamId = new("CONFIGURATION");
  }
  public ConfigurationId(StreamId streamId)
  {
    StreamId = streamId;
  }

  public static bool operator ==(ConfigurationId left, ConfigurationId right) => left.Equals(right);
  public static bool operator !=(ConfigurationId left, ConfigurationId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is ConfigurationId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
