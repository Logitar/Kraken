namespace Logitar.Kraken.Contracts.Users;

public record AddressModel : ContactModel, IAddress
{
  public string Street { get; set; }
  public string Locality { get; set; }
  public string? PostalCode { get; set; }
  public string? Region { get; set; }
  public string Country { get; set; }
  public string Formatted { get; set; }

  public AddressModel() : this(string.Empty, string.Empty, null, null, string.Empty, string.Empty)
  {
  }

  public AddressModel(IAddress address, string formatted) : this(address.Street, address.Locality, address.PostalCode, address.Region, address.Country, formatted)
  {
    IsVerified = address.IsVerified;
  }

  public AddressModel(string street, string locality, string? postalCode, string? region, string country, string formatted)
  {
    Street = street;
    Locality = locality;
    PostalCode = postalCode;
    Region = region;
    Country = country;
    Formatted = formatted;
  }
}
