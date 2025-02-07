using Logitar.EventSourcing;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Infrastructure.Converters;

internal class RealmIdConverter : JsonConverter<RealmId>
{
  public override RealmId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new RealmId() : new(new StreamId(value));
  }

  public override void Write(Utf8JsonWriter writer, RealmId realmId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(realmId.Value);
  }
}
