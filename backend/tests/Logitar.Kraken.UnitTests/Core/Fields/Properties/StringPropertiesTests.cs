using FluentValidation;
using Logitar.Kraken.Contracts.Fields;

namespace Logitar.Kraken.Core.Fields.Properties;

[Trait(Traits.Category, Categories.Unit)]
public class StringPropertiesTests
{
  [Fact(DisplayName = "It should construct the correct instance from another instance.")]
  public void Given_Instance_When_ctor_Then_Constructed()
  {
    StringPropertiesModel model = new()
    {
      MinimumLength = 1,
      MaximumLength = 100,
      Pattern = "[A-Z]{4}\\s?[0-9]{4}\\s?[0-9]{4}"
    };
    StringProperties properties = new(model);
    Assert.Equal(model.MinimumLength, properties.MinimumLength);
    Assert.Equal(model.MaximumLength, properties.MaximumLength);
    Assert.Equal(model.Pattern, properties.Pattern);
  }

  [Fact(DisplayName = "It should construct the correct instance with arguments.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    int minimumLength = 1;
    int maximumLength = 100;
    string pattern = "[A-Z]{4}\\s?[0-9]{4}\\s?[0-9]{4}";
    StringProperties properties = new(minimumLength, maximumLength, pattern);
    Assert.Equal(minimumLength, properties.MinimumLength);
    Assert.Equal(maximumLength, properties.MaximumLength);
    Assert.Equal(pattern, properties.Pattern);
  }

  [Fact(DisplayName = "It should construct the correct instance without argument.")]
  public void Given_NoArgument_When_ctor_Then_Constructed()
  {
    StringProperties properties = new();
    Assert.Null(properties.MinimumLength);
    Assert.Null(properties.MaximumLength);
    Assert.Null(properties.Pattern);
  }

  [Fact(DisplayName = "It should return the correct data type.")]
  public void Given_StringProperties_When_getDataType_Then_CorrectDataType()
  {
    Assert.Equal(DataType.String, new StringProperties().DataType);
  }

  [Fact(DisplayName = "It should throw ValidationException when the arguments are not valid.")]
  public void Given_InvalidArguments_When_ctor_Then_ValidationException()
  {
    int minimumLength = 100;
    int maximumLength = 1;
    string pattern = "    ";
    var exception = Assert.Throws<ValidationException>(() => new StringProperties(minimumLength, maximumLength, pattern));
    Assert.Equal(3, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "LessThanOrEqualValidator" && e.PropertyName == "MinimumLength.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanOrEqualValidator" && e.PropertyName == "MaximumLength.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Pattern");
  }
}
