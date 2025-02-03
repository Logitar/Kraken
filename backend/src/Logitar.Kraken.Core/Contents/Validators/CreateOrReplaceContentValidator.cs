using FluentValidation;
using Logitar.Kraken.Contracts.Contents;

namespace Logitar.Kraken.Core.Contents.Validators;

internal class CreateOrReplaceContentValidator : AbstractValidator<CreateOrReplaceContentPayload>
{
  public CreateOrReplaceContentValidator()
  {
    RuleFor(x => x.UniqueName).UniqueName(Content.UniqueNameSettings);
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
  }
}
