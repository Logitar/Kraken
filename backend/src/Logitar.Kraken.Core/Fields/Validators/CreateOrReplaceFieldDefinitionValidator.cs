using FluentValidation;
using Logitar.Kraken.Contracts.Fields;

namespace Logitar.Kraken.Core.Fields.Validators;

internal class CreateOrReplaceFieldDefinitionValidator : AbstractValidator<CreateOrReplaceFieldDefinitionPayload>
{
  public CreateOrReplaceFieldDefinitionValidator()
  {
    RuleFor(x => x.UniqueName).Identifier();
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
    When(x => !string.IsNullOrWhiteSpace(x.Placeholder), () => RuleFor(x => x.Placeholder!).Placeholder());
  }
}
