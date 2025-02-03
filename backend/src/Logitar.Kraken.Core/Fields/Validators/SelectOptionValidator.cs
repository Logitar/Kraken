using FluentValidation;
using Logitar.Kraken.Contracts.Fields;

namespace Logitar.Kraken.Core.Fields.Validators;

internal class SelectOptionValidator : AbstractValidator<SelectOptionModel>
{
  public SelectOptionValidator()
  {
    RuleFor(x => x.Text).NotEmpty();
  }
}
