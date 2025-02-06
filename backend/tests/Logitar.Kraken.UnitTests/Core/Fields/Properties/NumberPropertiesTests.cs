using FluentValidation;
using Logitar.Kraken.Contracts.Fields;

namespace Logitar.Kraken.Core.Fields.Properties;

[Trait(Traits.Category, Categories.Unit)]
public class NumberPropertiesTests
{
  [Fact(DisplayName = "It should construct the correct instance from another instance.")]
  public void Given_Instance_When_ctor_Then_Constructed()
  {
    NumberPropertiesModel model = new()
    {
      MinimumValue = 0.0000,
      MaximumValue = 1.0000,
      Step = 0.0001
    };
    NumberProperties properties = new(model);
    Assert.Equal(model.MinimumValue, properties.MinimumValue);
    Assert.Equal(model.MaximumValue, properties.MaximumValue);
  }

  [Fact(DisplayName = "It should construct the correct instance with arguments.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    double minimumValue = 0.0000;
    double maximumValue = 1.0000;
    double step = 0.0001;
    NumberProperties properties = new(minimumValue, maximumValue, step);
    Assert.Equal(minimumValue, properties.MinimumValue);
    Assert.Equal(maximumValue, properties.MaximumValue);
    Assert.Equal(step, properties.Step);
  }

  [Fact(DisplayName = "It should construct the correct instance without argument.")]
  public void Given_NoArgument_When_ctor_Then_Constructed()
  {
    NumberProperties properties = new();
    Assert.Null(properties.MinimumValue);
    Assert.Null(properties.MaximumValue);
    Assert.Null(properties.Step);
  }

  [Fact(DisplayName = "It should return the correct data type.")]
  public void Given_NumberProperties_When_getDataType_Then_CorrectDataType()
  {
    Assert.Equal(DataType.Number, new NumberProperties().DataType);
  }

  [Fact(DisplayName = "It should throw ValidationException when the arguments are not valid.")]
  public void Given_InvalidArguments_When_ctor_Then_ValidationException()
  {
    double minimumValue = 1.0000;
    double maximumValue = 0.0000;
    double step = 1.0000;
    var exception = Assert.Throws<ValidationException>(() => new NumberProperties(minimumValue, maximumValue, step));
    Assert.Equal(4, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "LessThanOrEqualValidator" && e.PropertyName == "MinimumValue.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanOrEqualValidator" && e.PropertyName == "MaximumValue.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanValidator" && e.PropertyName == "MaximumValue.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "LessThanValidator" && e.PropertyName == "Step.Value");
  }
}
