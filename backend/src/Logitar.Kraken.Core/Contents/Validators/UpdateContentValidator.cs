using FluentValidation;
using Logitar.Kraken.Contracts.Contents;

namespace Logitar.Kraken.Core.Contents.Validators;

internal class UpdateContentValidator : AbstractValidator<UpdateContentPayload>
{
  public UpdateContentValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).UniqueName(Content.UniqueNameSettings));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());
  }
}
