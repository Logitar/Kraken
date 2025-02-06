using FluentValidation;
using Logitar.Kraken.Contracts.Fields;
using System.Net.Mime; // NOTE(fpion): cannot be added to CSPROJ due to ContentType aggregate.

namespace Logitar.Kraken.Core.Fields.Properties;

[Trait(Traits.Category, Categories.Unit)]
public class RichTextPropertiesTests
{
  [Fact(DisplayName = "It should construct the correct instance from another instance.")]
  public void Given_Instance_When_ctor_Then_Constructed()
  {
    RichTextPropertiesModel model = new()
    {
      ContentType = MediaTypeNames.Text.Plain,
      MinimumLength = 1,
      MaximumLength = 1000
    };
    RichTextProperties properties = new(model);
    Assert.Equal(model.ContentType, properties.ContentType);
    Assert.Equal(model.MinimumLength, properties.MinimumLength);
    Assert.Equal(model.MaximumLength, properties.MaximumLength);
  }

  [Fact(DisplayName = "It should construct the correct instance with arguments.")]
  public void Given_Arguments_When_ctor_Then_Constructed()
  {
    string contentType = MediaTypeNames.Text.Plain;
    int minimumLength = 1;
    int maximumLength = 1000;
    RichTextProperties properties = new(contentType, minimumLength, maximumLength);
    Assert.Equal(contentType, properties.ContentType);
    Assert.Equal(minimumLength, properties.MinimumLength);
    Assert.Equal(maximumLength, properties.MaximumLength);
  }

  [Fact(DisplayName = "It should construct the correct instance without argument.")]
  public void Given_NoArgument_When_ctor_Then_Constructed()
  {
    RichTextProperties properties = new(MediaTypeNames.Text.Plain);
    Assert.Null(properties.MinimumLength);
    Assert.Null(properties.MaximumLength);
  }

  [Fact(DisplayName = "It should return the correct data type.")]
  public void Given_RichTextProperties_When_getDataType_Then_CorrectDataType()
  {
    Assert.Equal(DataType.RichText, new RichTextProperties(MediaTypeNames.Text.Plain).DataType);
  }

  [Fact(DisplayName = "It should throw ValidationException when the arguments are not valid.")]
  public void Given_InvalidArguments_When_ctor_Then_ValidationException()
  {
    string contentType = MediaTypeNames.Text.Markdown;
    int minimumLength = 1000;
    int maximumLength = 1;
    var exception = Assert.Throws<ValidationException>(() => new RichTextProperties(contentType, minimumLength, maximumLength));
    Assert.Equal(3, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "ContentTypeValidator" && e.PropertyName == "ContentType");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "LessThanOrEqualValidator" && e.PropertyName == "MinimumLength.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanOrEqualValidator" && e.PropertyName == "MaximumLength.Value");
  }
}
