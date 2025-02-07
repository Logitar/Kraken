using Logitar.EventSourcing;
using Logitar.Kraken.Core.Templates;

namespace Logitar.Kraken.Infrastructure.Converters;

internal class TemplateIdConverter : JsonConverter<TemplateId>
{
  public override TemplateId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new TemplateId() : new(new StreamId(value));
  }

  public override void Write(Utf8JsonWriter writer, TemplateId templateId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(templateId.Value);
  }
}
