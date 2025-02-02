using FluentValidation;
using Logitar.Kraken.Contracts.Tokens;

namespace Logitar.Kraken.Core.Tokens.Validators;

internal class ClaimValidator : AbstractValidator<ClaimModel>
{
  public ClaimValidator()
  {
    RuleFor(x => x.Name).Identifier();
    RuleFor(x => x.Value).NotEmpty();
  }
}
