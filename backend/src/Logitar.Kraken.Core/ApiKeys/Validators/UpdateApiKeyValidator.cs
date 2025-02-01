using FluentValidation;
using Logitar.Kraken.Contracts.ApiKeys;
using Logitar.Kraken.Core.Roles.Validators;
using Logitar.Kraken.Core.Validators;

namespace Logitar.Kraken.Core.ApiKeys.Validators;

internal class UpdateApiKeyValidator : AbstractValidator<UpdateApiKeyPayload>
{
  public UpdateApiKeyValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());
    When(x => x.ExpiresOn.HasValue, () => RuleFor(x => x.ExpiresOn!.Value).Future());

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeValidator());
    RuleForEach(x => x.Roles).SetValidator(new RoleActionValidator());
  }
}
