using FluentValidation;
using Logitar.Kraken.Contracts.Passwords;
using Logitar.Kraken.Core.Validators;

namespace Logitar.Kraken.Core.Passwords.Validators;

internal class ValidateOneTimePasswordValidator : AbstractValidator<ValidateOneTimePasswordPayload>
{
  public ValidateOneTimePasswordValidator()
  {
    RuleFor(x => x.Password).NotEmpty();

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeValidator());
  }
}
