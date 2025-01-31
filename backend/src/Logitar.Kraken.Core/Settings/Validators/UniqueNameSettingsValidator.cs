using FluentValidation;
using Logitar.Kraken.Contracts.Settings;

namespace Logitar.Kraken.Core.Settings.Validators;

internal class UniqueNameSettingsValidator : AbstractValidator<IUniqueNameSettings>
{
  public UniqueNameSettingsValidator()
  {
    When(x => x.AllowedCharacters != null, () => RuleFor(x => x.AllowedCharacters).NotEmpty().MaximumLength(byte.MaxValue));
  }
}
