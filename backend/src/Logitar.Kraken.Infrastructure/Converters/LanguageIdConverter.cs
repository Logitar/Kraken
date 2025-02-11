using Logitar.EventSourcing;
using Logitar.Kraken.Core.Localization;

namespace Logitar.Kraken.Infrastructure.Converters;

internal class LanguageIdConverter : JsonConverter<LanguageId>
{
  public override LanguageId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new LanguageId() : new(new StreamId(value));
  }

  public override void Write(Utf8JsonWriter writer, LanguageId languageId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(languageId.Value);
  }
}
