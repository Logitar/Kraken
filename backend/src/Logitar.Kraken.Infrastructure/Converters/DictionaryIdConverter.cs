using Logitar.EventSourcing;
using Logitar.Kraken.Core.Dictionaries;

namespace Logitar.Kraken.Infrastructure.Converters;

internal class DictionaryIdConverter : JsonConverter<DictionaryId>
{
  public override DictionaryId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new DictionaryId() : new(new StreamId(value));
  }

  public override void Write(Utf8JsonWriter writer, DictionaryId dictionaryId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(dictionaryId.Value);
  }
}
