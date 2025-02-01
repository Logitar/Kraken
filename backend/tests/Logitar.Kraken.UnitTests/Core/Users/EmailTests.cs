using System.Text.Json;

namespace Logitar.Kraken.Core.Users;

[Trait(Traits.Category, Categories.Unit)]
public class EmailTests
{
  [Fact(DisplayName = "It should be deserialized correctly.")]
  public void Given_EmailAddress_When_Deserialize_Then_Deserialized()
  {
    string address = "info@theatrebeanfield.ca";
    bool isVerified = true;
    string json = string.Concat(
      '{',
      $@"""Address"":""{address}"",",
      $@"""IsVerified"":{isVerified.ToString().ToLowerInvariant()}",
      '}');

    Email? email = JsonSerializer.Deserialize<Email>(json);

    Assert.NotNull(email);
    Assert.Equal(address, email.Address);
    Assert.Equal(isVerified, email.IsVerified);
  }

  [Fact(DisplayName = "It should deserialize null correctly.")]
  public void Given_Null_When_Deserialize_Then_Null()
  {
    Assert.Null(JsonSerializer.Deserialize<Email>("null"));
  }

  [Fact(DisplayName = "It should be serialized correctly.")]
  public void Given_EmailAddress_When_Serialize_Then_Serialized()
  {
    string address = "info@theatrebeanfield.ca";
    bool isVerified = true;
    Email email = new(address, isVerified);

    string json = JsonSerializer.Serialize(email);

    Assert.Equal(string.Concat(
      '{',
      $@"""Address"":""{address}"",",
      $@"""IsVerified"":{isVerified.ToString().ToLowerInvariant()}",
      '}'), json);
  }
}
