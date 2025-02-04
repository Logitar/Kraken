using FluentValidation;
using Logitar.Kraken.Contracts.Templates;

namespace Logitar.Kraken.Core.Templates.Validators;

internal class CreateOrReplaceTemplateValidator : AbstractValidator<CreateOrReplaceTemplatePayload>
{
  public CreateOrReplaceTemplateValidator()
  {
    RuleFor(x => x.UniqueKey).Identifier();
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleFor(x => x.Subject).Subject();
    RuleFor(x => x.Content).SetValidator(new TemplateContentValidator());
  }
}
