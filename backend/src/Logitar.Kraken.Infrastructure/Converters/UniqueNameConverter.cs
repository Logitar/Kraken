using Logitar.Kraken.Core;
using Logitar.Kraken.Core.Settings;

namespace Logitar.Kraken.Infrastructure.Converters;

internal class UniqueNameConverter : JsonConverter<UniqueName>
{
  private readonly UniqueNameSettings _settings = new(allowedCharacters: null);

  public override UniqueName? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? null : new(_settings, value);
  }

  public override void Write(Utf8JsonWriter writer, UniqueName uniqueName, JsonSerializerOptions options)
  {
    writer.WriteStringValue(uniqueName.Value);
  }
}
