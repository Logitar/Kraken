using FluentValidation;
using Logitar.Kraken.Contracts.Roles;
using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Core.Validators;

namespace Logitar.Kraken.Core.Roles.Validators;

internal class CreateOrReplaceRoleValidator : AbstractValidator<CreateOrReplaceRolePayload>
{
  public CreateOrReplaceRoleValidator(IUniqueNameSettings uniqueNameSettings)
  {
    RuleFor(x => x.UniqueName).UniqueName(uniqueNameSettings);
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeValidator());
  }
}
