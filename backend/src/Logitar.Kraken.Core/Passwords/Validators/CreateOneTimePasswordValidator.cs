using FluentValidation;
using Logitar.Kraken.Contracts.Passwords;
using Logitar.Kraken.Core.Validators;

namespace Logitar.Kraken.Core.Passwords.Validators;

internal class CreateOneTimePasswordValidator : AbstractValidator<CreateOneTimePasswordPayload>
{
  public CreateOneTimePasswordValidator()
  {
    RuleFor(x => x.Characters).NotEmpty();
    RuleFor(x => x.Length).GreaterThan(0);

    When(x => x.ExpiresOn.HasValue, () => RuleFor(x => x.ExpiresOn!.Value).Future());
    When(x => x.MaximumAttempts.HasValue, () => RuleFor(x => x.MaximumAttempts!.Value).GreaterThan(0));

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeValidator());
  }
}
