namespace Logitar.Kraken.Core.Users;

[Trait(Traits.Category, Categories.Unit)]
public class PhoneSerializationTests
{
  [Fact(DisplayName = "It should be deserialized correctly.")]
  public void Given_PhoneNumber_When_Deserialize_Then_Deserialized()
  {
    string countryCode = "CA";
    string number = "514-492-1775";
    string extension = "123456";
    bool isVerified = true;
    string json = string.Concat(
      '{',
      $@"""CountryCode"":""{countryCode}"",",
      $@"""Number"":""{number}"",",
      $@"""Extension"":""{extension}"",",
      $@"""IsVerified"":{isVerified.ToString().ToLowerInvariant()}",
      '}');

    Phone? phone = JsonSerializer.Deserialize<Phone>(json);

    Assert.NotNull(phone);
    Assert.Equal(countryCode, phone.CountryCode);
    Assert.Equal(number, phone.Number);
    Assert.Equal(extension, phone.Extension);
    Assert.Equal(isVerified, phone.IsVerified);
  }

  [Fact(DisplayName = "It should deserialize null correctly.")]
  public void Given_Null_When_Deserialize_Then_Null()
  {
    Assert.Null(JsonSerializer.Deserialize<Phone>("null"));
  }

  [Fact(DisplayName = "It should be serialized correctly.")]
  public void Given_PhoneNumber_When_Serialize_Then_Serialized()
  {
    string countryCode = "CA";
    string number = "514-492-1775";
    string extension = "123456";
    bool isVerified = true;
    Phone phone = new(number, countryCode, extension, isVerified);

    string json = JsonSerializer.Serialize(phone);

    Assert.Equal(string.Concat(
      '{',
      $@"""CountryCode"":""{countryCode}"",",
      $@"""Number"":""{number}"",",
      $@"""Extension"":""{extension}"",",
      $@"""IsVerified"":{isVerified.ToString().ToLowerInvariant()}",
      '}'), json);
  }
}
