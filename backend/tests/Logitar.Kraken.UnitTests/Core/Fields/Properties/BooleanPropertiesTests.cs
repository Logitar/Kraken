using Logitar.Kraken.Contracts.Fields;

namespace Logitar.Kraken.Core.Fields.Properties;

[Trait(Traits.Category, Categories.Unit)]
public class BooleanPropertiesTests
{
  [Fact(DisplayName = "It should construct the correct instance from another instance.")]
  public void Given_Instance_When_ctor_Then_Constructed()
  {
    _ = new BooleanProperties(new BooleanPropertiesModel());
  }

  [Fact(DisplayName = "It should construct the correct instance without argument.")]
  public void Given_NoArgument_When_ctor_Then_Constructed()
  {
    _ = new BooleanProperties();
  }

  [Fact(DisplayName = "It should return the correct data type.")]
  public void Given_BooleanProperties_When_getDataType_Then_CorrectDataType()
  {
    Assert.Equal(DataType.Boolean, new BooleanProperties().DataType);
  }
}
