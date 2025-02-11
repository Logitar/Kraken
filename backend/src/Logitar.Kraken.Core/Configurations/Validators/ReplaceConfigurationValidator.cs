using FluentValidation;
using Logitar.Kraken.Contracts.Configurations;
using Logitar.Kraken.Core.Settings.Validators;

namespace Logitar.Kraken.Core.Configurations.Validators;

internal class ReplaceConfigurationValidator : AbstractValidator<ReplaceConfigurationPayload>
{
  public ReplaceConfigurationValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Secret), () => RuleFor(x => x.Secret!).Secret());

    RuleFor(x => x.UniqueNameSettings).SetValidator(new UniqueNameSettingsValidator());
    RuleFor(x => x.PasswordSettings).SetValidator(new PasswordSettingsValidator());
  }
}
