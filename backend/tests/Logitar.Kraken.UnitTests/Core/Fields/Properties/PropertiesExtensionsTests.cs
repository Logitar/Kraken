using Logitar.Kraken.Contracts.Fields;
using Logitar.Kraken.Core.Realms;

namespace Logitar.Kraken.Core.Fields.Properties;

[Trait(Traits.Category, Categories.Unit)]
public class PropertiesExtensionsTests
{
  [Theory(DisplayName = "ToRelatedContentProperties: it should return the correct properties from a model.")]
  [InlineData(null)]
  [InlineData("774df6eb-33fd-40ff-8a5f-bcc8936be151")]
  public void Given_Model_When_ToRelatedContentProperties_Then_Properties(string? realmIdValue)
  {
    RealmId? realmId = realmIdValue == null ? null : new(Guid.Parse(realmIdValue));
    RelatedContentPropertiesModel model = new()
    {
      ContentTypeId = Guid.NewGuid(),
      IsMultiple = true
    };

    RelatedContentProperties properties = model.ToRelatedContentProperties(realmId);

    Assert.Equal(realmId, properties.ContentTypeId.RealmId);
    Assert.Equal(model.ContentTypeId, properties.ContentTypeId.EntityId);
    Assert.Equal(model.IsMultiple, properties.IsMultiple);
  }

  [Fact(DisplayName = "ToSelectProperties: it should return the correct properties from a model.")]
  public void Given_Model_When_ToSelectProperties_Then_Properties()
  {
    SelectPropertiesModel model = new()
    {
      IsMultiple = true,
      Options =
      [
        new SelectOptionModel
        {
          Text = "Red",
          IsDisabled = true,
          Value = " red "
        },
        new SelectOptionModel
        {
          Text = "Gr33n",
          Label = "Green"
        },
        new SelectOptionModel
        {
          Text = "Blue",
          Value = "    "
        }
      ]
    };

    SelectProperties properties = model.ToSelectProperties();

    Assert.Equal(model.IsMultiple, properties.IsMultiple);

    Assert.Equal(model.Options.Count, properties.Options.Count);
    foreach (SelectOptionModel option in model.Options)
    {
      Assert.Contains(properties.Options, o => o.Text == option.Text.Trim()
        && o.IsDisabled == option.IsDisabled
        && o.Label == option.Label?.CleanTrim()
        && o.Value == option.Value?.CleanTrim());
    }
  }
}
