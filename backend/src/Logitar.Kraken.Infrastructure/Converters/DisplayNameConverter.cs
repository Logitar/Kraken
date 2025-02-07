﻿using Logitar.Kraken.Core;

namespace Logitar.Kraken.Infrastructure.Converters;

internal class DisplayNameConverter : JsonConverter<DisplayName>
{
  public override DisplayName? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? null : new(value);
  }

  public override void Write(Utf8JsonWriter writer, DisplayName displayName, JsonSerializerOptions options)
  {
    writer.WriteStringValue(displayName.Value);
  }
}
