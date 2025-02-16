using FluentValidation;
using Logitar.Kraken.Contracts.Sessions;
using Logitar.Kraken.Core.Validators;

namespace Logitar.Kraken.Core.Sessions.Validators;

internal class SignInSessionValidator : AbstractValidator<SignInSessionPayload>
{
  public SignInSessionValidator()
  {
    RuleFor(x => x.UniqueName).NotEmpty();
    RuleFor(x => x.Password).NotEmpty();

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeValidator());
  }
}
