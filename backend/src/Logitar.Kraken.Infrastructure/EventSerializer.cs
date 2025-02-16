using Logitar.Kraken.Infrastructure.Converters;

namespace Logitar.Kraken.Infrastructure;

public class EventSerializer : EventSourcing.Infrastructure.EventSerializer
{
  public EventSerializer(PasswordConverter passwordConverter)
  {
    SerializerOptions.Converters.Add(passwordConverter);
  }

  protected override void RegisterConverters()
  {
    base.RegisterConverters();

    SerializerOptions.Converters.Add(new ConfigurationIdConverter());
    SerializerOptions.Converters.Add(new DescriptionConverter());
    SerializerOptions.Converters.Add(new DisplayNameConverter());
    SerializerOptions.Converters.Add(new LanguageIdConverter());
    SerializerOptions.Converters.Add(new LocaleConverter());
    SerializerOptions.Converters.Add(new RealmIdConverter());
    SerializerOptions.Converters.Add(new SecretConverter());
    SerializerOptions.Converters.Add(new SlugConverter());
    SerializerOptions.Converters.Add(new UniqueNameConverter());
    SerializerOptions.Converters.Add(new UrlConverter());
    SerializerOptions.Converters.Add(new UserIdConverter());
  }
}
