using Logitar.Kraken.Core.Templates;

namespace Logitar.Kraken.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class SubjectConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly Subject _subject = new("Reset your password");

  public SubjectConverterTests()
  {
    _serializerOptions.Converters.Add(new SubjectConverter());
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _subject, '"');
    Subject? subject = JsonSerializer.Deserialize<Subject?>(json, _serializerOptions);
    Assert.NotNull(subject);
    Assert.Equal(_subject, subject);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    Subject? subject = JsonSerializer.Deserialize<Subject?>("null", _serializerOptions);
    Assert.Null(subject);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_subject, _serializerOptions);
    Assert.Equal(string.Concat('"', _subject, '"'), json);
  }
}
