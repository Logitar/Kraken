using FluentValidation;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core.Users.Validators;

namespace Logitar.Kraken.Core.Users;

public record Phone : Contact, IPhone
{
  public const int CountryCodeMaximumLength = 2;
  public const int NumberMaximumLength = 20;
  public const int ExtensionMaximumLength = 10;

  public string? CountryCode { get; }
  public string Number { get; }
  public string? Extension { get; }

  public Phone(string number, string? countryCode = null, string? extension = null, bool isVerified = false) : base(isVerified)
  {
    CountryCode = countryCode?.CleanTrim();
    Number = number.Trim();
    Extension = extension?.CleanTrim();
    new PhoneValidator().ValidateAndThrow(this);
  }

  public override string ToString() => this.FormatToE164();
}
