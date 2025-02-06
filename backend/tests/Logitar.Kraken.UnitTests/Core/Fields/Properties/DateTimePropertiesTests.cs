using FluentValidation;
using Logitar.Kraken.Contracts.Fields;

namespace Logitar.Kraken.Core.Fields.Properties;

[Trait(Traits.Category, Categories.Unit)]
public class DateTimePropertiesTests
{
  [Fact(DisplayName = "It should construct the correct instance from another instance.")]
  public void Given_Instance_When_ctor_Then_Constructed()
  {
    DateTimePropertiesModel model = new()
    {
      MinimumValue = new DateTime(2000, 1, 1),
      MaximumValue = new DateTime(2009, 12, 31, 23, 59, 59)
    };
    DateTimeProperties properties = new(model);
    Assert.Equal(model.MinimumValue, properties.MinimumValue);
    Assert.Equal(model.MaximumValue, properties.MaximumValue);
  }

  [Fact(DisplayName = "It should construct the correct instance with arguments.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    DateTime minimumValue = new(2000, 1, 1);
    DateTime maximumValue = new(2009, 12, 31, 23, 59, 59);
    DateTimeProperties properties = new(minimumValue, maximumValue);
    Assert.Equal(minimumValue, properties.MinimumValue);
    Assert.Equal(maximumValue, properties.MaximumValue);
  }

  [Fact(DisplayName = "It should construct the correct instance without argument.")]
  public void Given_NoArgument_When_ctor_Then_Constructed()
  {
    DateTimeProperties properties = new();
    Assert.Null(properties.MinimumValue);
    Assert.Null(properties.MaximumValue);
  }

  [Fact(DisplayName = "It should return the correct data type.")]
  public void Given_DateTimeProperties_When_getDataType_Then_CorrectDataType()
  {
    Assert.Equal(DataType.DateTime, new DateTimeProperties().DataType);
  }

  [Fact(DisplayName = "It should throw ValidationException when the arguments are not valid.")]
  public void Given_InvalidArguments_When_ctor_Then_ValidationException()
  {
    DateTime minimumValue = new(2009, 12, 31, 23, 59, 59);
    DateTime maximumValue = new(2000, 1, 1);
    var exception = Assert.Throws<ValidationException>(() => new DateTimeProperties(minimumValue, maximumValue));
    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "LessThanOrEqualValidator" && e.PropertyName == "MinimumValue.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanOrEqualValidator" && e.PropertyName == "MaximumValue.Value");
  }
}
