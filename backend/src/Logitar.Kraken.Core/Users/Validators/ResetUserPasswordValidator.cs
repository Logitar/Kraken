using FluentValidation;
using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Contracts.Users;

namespace Logitar.Kraken.Core.Users.Validators;

internal class ResetUserPasswordValidator : AbstractValidator<ResetUserPasswordPayload>
{
  public ResetUserPasswordValidator(IPasswordSettings passwordSettings)
  {
    RuleFor(x => x.Password).Password(passwordSettings);
  }
}
