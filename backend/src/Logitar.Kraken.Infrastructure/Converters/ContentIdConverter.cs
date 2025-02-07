using Logitar.EventSourcing;
using Logitar.Kraken.Core.Contents;

namespace Logitar.Kraken.Infrastructure.Converters;

internal class ContentIdConverter : JsonConverter<ContentId>
{
  public override ContentId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new ContentId() : new(new StreamId(value));
  }

  public override void Write(Utf8JsonWriter writer, ContentId contentId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(contentId.Value);
  }
}
