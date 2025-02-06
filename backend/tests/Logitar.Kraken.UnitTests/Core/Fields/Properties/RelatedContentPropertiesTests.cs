using Logitar.Kraken.Core.Contents;

namespace Logitar.Kraken.Core.Fields.Properties;

[Trait(Traits.Category, Categories.Unit)]
public class RelatedContentPropertiesTests
{
  [Fact(DisplayName = "It should construct the correct instance from another instance.")]
  public void Given_Instance_When_ctor_Then_Constructed()
  {
    RelatedContentProperties source = new(ContentTypeId.NewId(realmId: null));
    RelatedContentProperties properties = new(source);
    Assert.Equal(source.ContentTypeId, properties.ContentTypeId);
    Assert.Equal(source.IsMultiple, properties.IsMultiple);
  }

  [Fact(DisplayName = "It should construct the correct instance with arguments.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    ContentTypeId contentTypeId = ContentTypeId.NewId(realmId: null);
    bool isMultiple = true;
    RelatedContentProperties properties = new(contentTypeId, isMultiple);
    Assert.Equal(contentTypeId, properties.ContentTypeId);
    Assert.Equal(isMultiple, properties.IsMultiple);
  }
}
