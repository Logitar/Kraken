using Logitar.Kraken.Infrastructure.Converters;

namespace Logitar.Kraken.Infrastructure;

public class EventSerializer : EventSourcing.Infrastructure.EventSerializer
{
  protected override void RegisterConverters()
  {
    base.RegisterConverters();

    SerializerOptions.Converters.Add(new ConfigurationIdConverter());
    SerializerOptions.Converters.Add(new JwtSecretConverter());
  }
}
