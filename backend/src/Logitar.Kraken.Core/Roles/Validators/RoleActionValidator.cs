using FluentValidation;
using Logitar.Kraken.Contracts.Roles;

namespace Logitar.Kraken.Core.Roles.Validators;

internal class RoleActionValidator : AbstractValidator<RoleAction>
{
  public RoleActionValidator()
  {
    RuleFor(x => x.Role).NotEmpty();
    RuleFor(x => x.Action).IsInEnum();
  }
}
