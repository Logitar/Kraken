using FluentValidation;
using Logitar.Kraken.Contracts.Messages;

namespace Logitar.Kraken.Core.Messages.Validators;

internal class VariableValidator : AbstractValidator<Variable>
{
  public VariableValidator()
  {
    RuleFor(x => x.Key).Identifier();
  }
}
