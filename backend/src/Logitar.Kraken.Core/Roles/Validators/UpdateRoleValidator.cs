using FluentValidation;
using Logitar.Kraken.Contracts.Roles;
using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Core.Validators;

namespace Logitar.Kraken.Core.Roles.Validators;

internal class UpdateRoleValidator : AbstractValidator<UpdateRolePayload>
{
  public UpdateRoleValidator(IUniqueNameSettings uniqueNameSettings)
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).UniqueName(uniqueNameSettings));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeValidator());
  }
}
