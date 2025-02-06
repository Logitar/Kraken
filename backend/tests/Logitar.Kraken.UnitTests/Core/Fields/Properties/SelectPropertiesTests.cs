namespace Logitar.Kraken.Core.Fields.Properties;

[Trait(Traits.Category, Categories.Unit)]
public class SelectPropertiesTests
{
  [Fact(DisplayName = "It should construct the correct instance from another instance.")]
  public void Given_Instance_When_ctor_Then_Constructed()
  {
    SelectOption[] options = [new("No"), new("Yes")];
    SelectProperties source = new(isMultiple: false, options);
    SelectProperties properties = new(source);
    Assert.Equal(source.IsMultiple, properties.IsMultiple);
    Assert.Same(source.Options, properties.Options);
  }

  [Fact(DisplayName = "It should construct the correct instance with arguments.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    bool isMultiple = true;
    SelectOption[] options = [new("Blue"), new("Green"), new("Red")];
    SelectProperties properties = new(isMultiple, options);
    Assert.Equal(isMultiple, properties.IsMultiple);
    Assert.Same(options, properties.Options);
  }
}
