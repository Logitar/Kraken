using FluentValidation;
using Logitar.Kraken.Contracts.Templates;

namespace Logitar.Kraken.Core.Templates.Validators;

internal class TemplateContentValidator : AbstractValidator<ITemplateContent>
{
  public TemplateContentValidator()
  {
    RuleFor(x => x.Type).ContentType();
    RuleFor(x => x.Text).NotEmpty();
  }
}
