using FluentValidation;
using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Contracts.Users;

namespace Logitar.Kraken.Core.Users.Validators;

internal class ChangePasswordValidator : AbstractValidator<ChangePasswordPayload>
{
  public ChangePasswordValidator(IPasswordSettings passwordSettings)
  {
    When(x => x.Current != null, () => RuleFor(x => x.Current).NotEmpty());
    RuleFor(x => x.New).Password(passwordSettings);
  }
}
