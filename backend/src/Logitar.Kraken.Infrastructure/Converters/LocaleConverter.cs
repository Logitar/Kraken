using Logitar.Kraken.Core.Localization;

namespace Logitar.Kraken.Infrastructure.Converters;

internal class LocaleConverter : JsonConverter<Locale>
{
  public override Locale? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return Locale.TryCreate(value);
  }

  public override void Write(Utf8JsonWriter writer, Locale locale, JsonSerializerOptions options)
  {
    writer.WriteStringValue(locale.Code);
  }
}
