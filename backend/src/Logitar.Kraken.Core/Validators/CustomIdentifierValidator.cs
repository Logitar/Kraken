using FluentValidation;
using Logitar.Kraken.Contracts;

namespace Logitar.Kraken.Core.Validators;

internal class CustomIdentifierValidator : AbstractValidator<CustomIdentifierModel>
{
  public CustomIdentifierValidator()
  {
    RuleFor(x => x.Key).Identifier();
    RuleFor(x => x.Value).CustomIdentifier();
  }
}
