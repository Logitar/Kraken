using Logitar.EventSourcing;
using Logitar.Kraken.Core.Users;

namespace Logitar.Kraken.Infrastructure.Converters;

internal class UserIdConverter : JsonConverter<UserId>
{
  public override UserId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new UserId() : new(new StreamId(value));
  }

  public override void Write(Utf8JsonWriter writer, UserId userId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(userId.Value);
  }
}
