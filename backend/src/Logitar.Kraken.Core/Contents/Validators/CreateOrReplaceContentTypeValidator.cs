using FluentValidation;
using Logitar.Kraken.Contracts.Contents;

namespace Logitar.Kraken.Core.Contents.Validators;

internal class CreateOrReplaceContentTypeValidator : AbstractValidator<CreateOrReplaceContentTypePayload>
{
  public CreateOrReplaceContentTypeValidator()
  {
    RuleFor(x => x.UniqueName).Identifier();
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
  }
}
