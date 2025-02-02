using FluentValidation;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core.Users.Validators;

namespace Logitar.Kraken.Core.Users;

public record Address : Contact, IAddress
{
  public const int MaximumLength = byte.MaxValue;

  public string Street { get; }
  public string Locality { get; }
  public string? PostalCode { get; }
  public string? Region { get; }
  public string Country { get; }

  [JsonConstructor]
  public Address(string street, string locality, string country, string? region = null, string? postalCode = null, bool isVerified = false)
    : base(isVerified)
  {
    Street = street.Trim();
    Locality = locality.Trim();
    Region = region?.CleanTrim();
    PostalCode = postalCode?.CleanTrim();
    Country = country.Trim();
  }

  public Address(IAddressHelper helper, string street, string locality, string country, string? region = null, string? postalCode = null, bool isVerified = false)
    : this(street, locality, country, region, postalCode, isVerified)
  {
    new AddressValidator(helper).ValidateAndThrow(this);
  }

  public override string ToString()
  {
    StringBuilder formatted = new();
    string[] lines = Street.Remove("\r").Split('\n');
    foreach (string line in lines)
    {
      if (!string.IsNullOrWhiteSpace(line))
      {
        formatted.AppendLine(line.Trim());
      }
    }
    formatted.Append(Locality);
    if (Region != null)
    {
      formatted.Append(' ').Append(Region);
    }
    if (PostalCode != null)
    {
      formatted.Append(' ').Append(PostalCode);
    }
    formatted.AppendLine();
    formatted.Append(Country);
    return formatted.ToString();
  }
}
