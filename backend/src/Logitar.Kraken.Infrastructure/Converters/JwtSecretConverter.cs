using Logitar.Kraken.Core.Tokens;

namespace Logitar.Kraken.Infrastructure.Converters;

internal class JwtSecretConverter : JsonConverter<JwtSecret>
{
  public override JwtSecret? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? null : new(value);
  }

  public override void Write(Utf8JsonWriter writer, JwtSecret secret, JsonSerializerOptions options)
  {
    writer.WriteStringValue(secret.Value);
  }
}
