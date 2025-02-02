namespace Logitar.Kraken.Contracts.Users;

public record PhonePayload : ContactPayload, IPhone
{
  public string? CountryCode { get; set; }
  public string Number { get; set; } = string.Empty;
  public string? Extension { get; set; }

  public PhonePayload() : base()
  {
  }

  public PhonePayload(string? countryCode, string number, string? extension, bool isVerified) : base(isVerified)
  {
    CountryCode = countryCode;
    Number = number;
    Extension = extension;
  }
}
