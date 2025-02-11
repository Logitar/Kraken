using Logitar.Kraken.Core.Tokens;

namespace Logitar.Kraken.Infrastructure.Converters;

[Trait(Traits.Category, Categories.Unit)]
public class JwtSecretConverterTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  private readonly JwtSecret _secret = new("agwWZrQ9vEfYtncb8xXAMRJUH57m2Kzy");

  public JwtSecretConverterTests()
  {
    _serializerOptions.Converters.Add(new JwtSecretConverter());
  }

  [Fact(DisplayName = "It should deserialize the correct value.")]
  public void Given_Value_When_Read_Then_Deserialized()
  {
    string json = string.Concat('"', _secret, '"');
    JwtSecret? secret = JsonSerializer.Deserialize<JwtSecret?>(json, _serializerOptions);
    Assert.NotNull(secret);
    Assert.Equal(_secret, secret);
  }

  [Fact(DisplayName = "It should deserialize the null value.")]
  public void Given_Null_When_Read_Then_NullReturned()
  {
    JwtSecret? secret = JsonSerializer.Deserialize<JwtSecret?>("null", _serializerOptions);
    Assert.Null(secret);
  }

  [Fact(DisplayName = "It should serialize the correct value.")]
  public void Given_Value_When_Write_Then_Serialized()
  {
    string json = JsonSerializer.Serialize(_secret, _serializerOptions);
    Assert.Equal(string.Concat('"', _secret, '"'), json);
  }
}
