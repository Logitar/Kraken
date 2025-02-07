using Logitar.EventSourcing;
using Logitar.Kraken.Core.Messages;

namespace Logitar.Kraken.Infrastructure.Converters;

internal class MessageIdConverter : JsonConverter<MessageId>
{
  public override MessageId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new MessageId() : new(new StreamId(value));
  }

  public override void Write(Utf8JsonWriter writer, MessageId messageId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(messageId.Value);
  }
}
