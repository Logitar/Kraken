using System.Text.Json;

namespace Logitar.Kraken.Core.Users;

[Trait(Traits.Category, Categories.Unit)]
public class AddressTests
{
  [Fact(DisplayName = "It should be deserialized correctly.")]
  public void Given_PostalAddress_When_Deserialize_Then_Deserialized()
  {
    string street = "2490 Notre-Dame St W";
    string locality = "Montreal";
    string postalCode = "H3J 1N5";
    string region = "QC";
    string country = "CA";
    bool isVerified = false;
    string json = string.Concat(
      '{',
      $@"""Street"":""{street}"",",
      $@"""Locality"":""{locality}"",",
      $@"""PostalCode"":""{postalCode}"",",
      $@"""Region"":""{region}"",",
      $@"""Country"":""{country}"",",
      $@"""IsVerified"":{isVerified.ToString().ToLowerInvariant()}",
      '}');

    Address? address = JsonSerializer.Deserialize<Address>(json);

    Assert.NotNull(address);
    Assert.Equal(street, address.Street);
    Assert.Equal(locality, address.Locality);
    Assert.Equal(postalCode, address.PostalCode);
    Assert.Equal(region, address.Region);
    Assert.Equal(country, address.Country);
    Assert.Equal(isVerified, address.IsVerified);
  }

  [Fact(DisplayName = "It should deserialize null correctly.")]
  public void Given_Null_When_Deserialize_Then_Null()
  {
    Assert.Null(JsonSerializer.Deserialize<Address>("null"));
  }

  [Fact(DisplayName = "It should be serialized correctly.")]
  public void Given_PostalAddress_When_Serialize_Then_Serialized()
  {
    string street = "2490 Notre-Dame St W";
    string locality = "Montreal";
    string postalCode = "H3J 1N5";
    string region = "QC";
    string country = "CA";
    bool isVerified = false;
    Address address = new(street, locality, country, region, postalCode, isVerified);

    string json = JsonSerializer.Serialize(address);

    Assert.Equal(string.Concat(
      '{',
      $@"""Street"":""{street}"",",
      $@"""Locality"":""{locality}"",",
      $@"""PostalCode"":""{postalCode}"",",
      $@"""Region"":""{region}"",",
      $@"""Country"":""{country}"",",
      $@"""IsVerified"":{isVerified.ToString().ToLowerInvariant()}",
      '}'), json);
  }
}
