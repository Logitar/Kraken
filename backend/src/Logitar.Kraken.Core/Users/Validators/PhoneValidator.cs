using FluentValidation;
using Logitar.Kraken.Contracts.Users;

namespace Logitar.Kraken.Core.Users.Validators;

internal class PhoneValidator : AbstractValidator<IPhone>
{
  public PhoneValidator()
  {
    When(x => x.CountryCode != null, () => RuleFor(x => x.CountryCode).NotEmpty().Length(Phone.CountryCodeMaximumLength));
    RuleFor(x => x.Number).NotEmpty().MaximumLength(Phone.NumberMaximumLength);
    When(x => x.Extension != null, () => RuleFor(x => x.Extension).NotEmpty().MaximumLength(Phone.ExtensionMaximumLength));

    RuleFor(x => x).Must(phone => phone.IsValid())
      .WithErrorCode("PhoneValidator")
      .WithMessage("'{PropertyName}' must be a valid phone number.");
  }
}
