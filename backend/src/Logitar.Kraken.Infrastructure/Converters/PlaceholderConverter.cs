using Logitar.Kraken.Core.Fields;

namespace Logitar.Kraken.Infrastructure.Converters;

internal class PlaceholderConverter : JsonConverter<Placeholder>
{
  public override Placeholder? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return Placeholder.TryCreate(value);
  }

  public override void Write(Utf8JsonWriter writer, Placeholder placeholder, JsonSerializerOptions options)
  {
    writer.WriteStringValue(placeholder.Value);
  }
}
