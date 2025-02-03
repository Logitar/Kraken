using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Contents;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Fields.Properties;

public static class PropertiesExtensions
{
  public static RelatedContentProperties ToRelatedContentProperties(this RelatedContentPropertiesModel properties, RealmId? realmId)
  {
    ContentTypeId contentTypeId = new(realmId, properties.ContentTypeId);
    return new RelatedContentProperties(contentTypeId, properties.IsMultiple);
  }

  public static SelectProperties ToSelectProperties(this SelectPropertiesModel properties)
  {
    IReadOnlyCollection<SelectOption> options = properties.Options.Select(option => new SelectOption(option)).ToList().AsReadOnly();
    return new SelectProperties(properties.IsMultiple, options);
  }
}
