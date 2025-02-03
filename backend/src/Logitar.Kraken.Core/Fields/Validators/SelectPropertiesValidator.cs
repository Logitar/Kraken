using FluentValidation;
using Logitar.Kraken.Contracts.Fields;

namespace Logitar.Kraken.Core.Fields.Validators;

internal class SelectPropertiesValidator : AbstractValidator<SelectPropertiesModel>
{
  public SelectPropertiesValidator()
  {
    RuleForEach(x => x.Options).SetValidator(new SelectOptionValidator());
  }
}
