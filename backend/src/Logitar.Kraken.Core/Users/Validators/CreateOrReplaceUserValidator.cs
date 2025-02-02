using FluentValidation;
using Logitar.Kraken.Contracts.Users;
using Logitar.Kraken.Core.Settings;
using Logitar.Kraken.Core.Validators;

namespace Logitar.Kraken.Core.Users.Validators;

internal class CreateOrReplaceUserValidator : AbstractValidator<CreateOrReplaceUserPayload>
{
  public CreateOrReplaceUserValidator(IUserSettings userSettings, IAddressHelper addressHelper)
  {
    RuleFor(x => x.UniqueName).UniqueName(userSettings.UniqueName);
    When(x => x.Password != null, () => RuleFor(x => x.Password!).Password(userSettings.Password));

    When(x => x.Address != null, () => RuleFor(x => x.Address!).SetValidator(new AddressValidator(addressHelper)));
    When(x => x.Email != null, () => RuleFor(x => x.Email!).SetValidator(new EmailValidator()));
    When(x => x.Phone != null, () => RuleFor(x => x.Phone!).SetValidator(new PhoneValidator()));

    When(x => !string.IsNullOrWhiteSpace(x.FirstName), () => RuleFor(x => x.FirstName!).PersonName());
    When(x => !string.IsNullOrWhiteSpace(x.MiddleName), () => RuleFor(x => x.MiddleName!).PersonName());
    When(x => !string.IsNullOrWhiteSpace(x.LastName), () => RuleFor(x => x.LastName!).PersonName());
    When(x => !string.IsNullOrWhiteSpace(x.Nickname), () => RuleFor(x => x.Nickname!).PersonName());

    When(x => x.Birthdate.HasValue, () => RuleFor(x => x.Birthdate!.Value).Past());
    When(x => !string.IsNullOrWhiteSpace(x.Gender), () => RuleFor(x => x.Gender!).Gender());
    When(x => !string.IsNullOrWhiteSpace(x.Locale), () => RuleFor(x => x.Locale!).Locale());
    When(x => !string.IsNullOrWhiteSpace(x.TimeZone), () => RuleFor(x => x.TimeZone!).TimeZone());

    When(x => !string.IsNullOrWhiteSpace(x.Picture), () => RuleFor(x => x.Picture!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Profile), () => RuleFor(x => x.Profile!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Website), () => RuleFor(x => x.Website!).Url());

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeValidator());
    RuleForEach(x => x.Roles).NotEmpty();
  }
}
