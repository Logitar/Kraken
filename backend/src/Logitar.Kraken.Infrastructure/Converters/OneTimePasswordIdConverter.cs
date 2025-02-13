using Logitar.EventSourcing;
using Logitar.Kraken.Core.Passwords;

namespace Logitar.Kraken.Infrastructure.Converters;

internal class OneTimePasswordIdConverter : JsonConverter<OneTimePasswordId>
{
  public override OneTimePasswordId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new OneTimePasswordId() : new(new StreamId(value));
  }

  public override void Write(Utf8JsonWriter writer, OneTimePasswordId oneTimePasswordId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(oneTimePasswordId.Value);
  }
}
