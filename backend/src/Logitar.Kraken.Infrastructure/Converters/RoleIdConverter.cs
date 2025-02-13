using Logitar.EventSourcing;
using Logitar.Kraken.Core.Roles;

namespace Logitar.Kraken.Infrastructure.Converters;

internal class RoleIdConverter : JsonConverter<RoleId>
{
  public override RoleId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new RoleId() : new(new StreamId(value));
  }

  public override void Write(Utf8JsonWriter writer, RoleId roleId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(roleId.Value);
  }
}
