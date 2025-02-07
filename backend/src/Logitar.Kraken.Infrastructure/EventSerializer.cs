using Logitar.Kraken.Infrastructure.Converters;

namespace Logitar.Kraken.Infrastructure;

public class EventSerializer : EventSourcing.Infrastructure.EventSerializer
{
  protected override void RegisterConverters()
  {
    base.RegisterConverters();

    SerializerOptions.Converters.Add(new ConfigurationIdConverter());
    SerializerOptions.Converters.Add(new ContentIdConverter());
    SerializerOptions.Converters.Add(new ContentTypeIdConverter());
    SerializerOptions.Converters.Add(new DescriptionConverter());
    SerializerOptions.Converters.Add(new DictionaryIdConverter());
    SerializerOptions.Converters.Add(new DisplayNameConverter());
    SerializerOptions.Converters.Add(new FieldTypeIdConverter());
    SerializerOptions.Converters.Add(new JwtSecretConverter());
    SerializerOptions.Converters.Add(new LanguageIdConverter());
    SerializerOptions.Converters.Add(new LocaleConverter());
    SerializerOptions.Converters.Add(new MessageIdConverter());
    SerializerOptions.Converters.Add(new PlaceholderConverter());
    SerializerOptions.Converters.Add(new RealmIdConverter());
    SerializerOptions.Converters.Add(new RoleIdConverter());
    SerializerOptions.Converters.Add(new SenderIdConverter());
    SerializerOptions.Converters.Add(new SlugConverter());
    SerializerOptions.Converters.Add(new SubjectConverter());
    SerializerOptions.Converters.Add(new TemplateIdConverter());
    SerializerOptions.Converters.Add(new UniqueNameConverter());
    SerializerOptions.Converters.Add(new UrlConverter());
  }
}
