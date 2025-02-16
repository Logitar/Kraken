using Logitar.Kraken.Core;

namespace Logitar.Kraken.Infrastructure.Converters;

internal class CustomIdentifierConverter : JsonConverter<CustomIdentifier>
{
  public override CustomIdentifier? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return CustomIdentifier.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, CustomIdentifier customIdentifier, JsonSerializerOptions options)
  {
    writer.WriteStringValue(customIdentifier.Value);
  }
}
