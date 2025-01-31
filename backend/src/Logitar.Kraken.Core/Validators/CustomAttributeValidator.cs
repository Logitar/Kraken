using FluentValidation;
using Logitar.Kraken.Contracts;

namespace Logitar.Kraken.Core.Validators;

internal class CustomAttributeValidator : AbstractValidator<CustomAttributeModel>
{
  public CustomAttributeValidator()
  {
    RuleFor(x => x.Key).Identifier();
  }
}
