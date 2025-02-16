using FluentValidation;
using Logitar.Kraken.Contracts.Tokens;

namespace Logitar.Kraken.Core.Tokens.Validators;

internal class ValidateTokenValidator : AbstractValidator<ValidateTokenPayload>
{
  public ValidateTokenValidator()
  {
    RuleFor(x => x.Token).NotEmpty();

    When(x => !string.IsNullOrWhiteSpace(x.Secret), () => RuleFor(x => x.Secret!).Secret());
  }
}
