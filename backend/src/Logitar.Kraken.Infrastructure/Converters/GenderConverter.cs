using Logitar.Kraken.Core.Users;

namespace Logitar.Kraken.Infrastructure.Converters;

internal class GenderConverter : JsonConverter<Gender>
{
  public override Gender? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return Gender.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, Gender gender, JsonSerializerOptions options)
  {
    writer.WriteStringValue(gender.Value);
  }
}
