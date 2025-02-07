using Logitar.EventSourcing;
using Logitar.Kraken.Core.ApiKeys;

namespace Logitar.Kraken.Infrastructure.Converters;

internal class ApiKeyIdConverter : JsonConverter<ApiKeyId>
{
  public override ApiKeyId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new ApiKeyId() : new(new StreamId(value));
  }

  public override void Write(Utf8JsonWriter writer, ApiKeyId apiKeyId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(apiKeyId.Value);
  }
}
