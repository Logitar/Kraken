using Logitar.Kraken.Core.Users;

namespace Logitar.Kraken.Infrastructure.Converters;

internal class PersonNameConverter : JsonConverter<PersonName>
{
  public override PersonName? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return PersonName.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, PersonName personName, JsonSerializerOptions options)
  {
    writer.WriteStringValue(personName.Value);
  }
}
