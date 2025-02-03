using FluentValidation;
using Logitar.Kraken.Contracts.Localization;

namespace Logitar.Kraken.Core.Localization.Validators;

internal class UpdateLanguageValidator : AbstractValidator<UpdateLanguagePayload>
{
  public UpdateLanguageValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Locale), () => RuleFor(x => x.Locale!).Locale());
  }
}
