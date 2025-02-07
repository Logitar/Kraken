using Logitar.EventSourcing;
using Logitar.Kraken.Core.Configurations;

namespace Logitar.Kraken.Infrastructure.Converters;

internal class ConfigurationIdConverter : JsonConverter<ConfigurationId>
{
  public override ConfigurationId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new ConfigurationId() : new(new StreamId(value));
  }

  public override void Write(Utf8JsonWriter writer, ConfigurationId configurationId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(configurationId.Value);
  }
}
