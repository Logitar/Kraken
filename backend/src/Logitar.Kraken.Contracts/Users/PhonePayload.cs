namespace Logitar.Kraken.Contracts.Users;

public record PhonePayload : ContactPayload, IPhone
{
  public string? CountryCode { get; set; }
  public string Number { get; set; } = string.Empty;
  public string? Extension { get; set; }

  public PhonePayload() : base()
  {
  }

  public PhonePayload(IPhone phone) : this(phone.CountryCode, phone.Number, phone.Extension, phone.IsVerified)
  {
  }

  public PhonePayload(string? countryCode, string number, string? extension, bool isVerified) : base(isVerified)
  {
    CountryCode = countryCode;
    Number = number;
    Extension = extension;
  }
}
