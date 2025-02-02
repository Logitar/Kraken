namespace Logitar.Kraken.Contracts.Users;

public record PhoneModel : ContactModel, IPhone
{
  public string? CountryCode { get; set; }
  public string Number { get; set; }
  public string? Extension { get; set; }
  public string E164Formatted { get; set; }

  public PhoneModel() : this(null, string.Empty, null, string.Empty)
  {
  }

  public PhoneModel(string? countryCode, string number, string? extension, string e164Formatted)
  {
    CountryCode = countryCode;
    Number = number;
    Extension = extension;
    E164Formatted = e164Formatted;
  }
}
