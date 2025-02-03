using FluentValidation;
using Logitar.Kraken.Contracts.Localization;

namespace Logitar.Kraken.Core.Localization.Validators;

internal class CreateOrReplaceLanguageValidator : AbstractValidator<CreateOrReplaceLanguagePayload>
{
  public CreateOrReplaceLanguageValidator()
  {
    RuleFor(x => x.Locale).Locale();
  }
}
