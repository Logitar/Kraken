using Logitar.EventSourcing;
using Logitar.Kraken.Core.Sessions;

namespace Logitar.Kraken.Infrastructure.Converters;

internal class SessionIdConverter : JsonConverter<SessionId>
{
  public override SessionId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new SessionId() : new(new StreamId(value));
  }

  public override void Write(Utf8JsonWriter writer, SessionId sessionId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(sessionId.Value);
  }
}
