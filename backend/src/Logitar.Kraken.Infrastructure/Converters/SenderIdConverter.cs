using Logitar.EventSourcing;
using Logitar.Kraken.Core.Senders;

namespace Logitar.Kraken.Infrastructure.Converters;

internal class SenderIdConverter : JsonConverter<SenderId>
{
  public override SenderId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new SenderId() : new(new StreamId(value));
  }

  public override void Write(Utf8JsonWriter writer, SenderId senderId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(senderId.Value);
  }
}
