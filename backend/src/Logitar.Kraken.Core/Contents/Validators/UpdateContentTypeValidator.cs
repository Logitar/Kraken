using FluentValidation;
using Logitar.Kraken.Contracts.Contents;

namespace Logitar.Kraken.Core.Contents.Validators;

internal class UpdateContentTypeValidator : AbstractValidator<UpdateContentTypePayload>
{
  public UpdateContentTypeValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).Identifier());
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());
  }
}
