﻿using Logitar.Kraken.Core;

namespace Logitar.Kraken.Infrastructure.Converters;

internal class UrlConverter : JsonConverter<Url>
{
  public override Url? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return Url.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, Url url, JsonSerializerOptions options)
  {
    writer.WriteStringValue(url.Value);
  }
}
