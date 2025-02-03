using FluentValidation;
using Logitar.Kraken.Contracts.Fields;

namespace Logitar.Kraken.Core.Fields.Validators;

internal class UpdateFieldDefinitionValidator : AbstractValidator<UpdateFieldDefinitionPayload>
{
  public UpdateFieldDefinitionValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).Identifier());
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());
    When(x => !string.IsNullOrWhiteSpace(x.Placeholder?.Value), () => RuleFor(x => x.Placeholder!.Value!).Placeholder());
  }
}
