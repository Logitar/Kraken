using FluentValidation;
using Logitar.Kraken.Contracts.Sessions;
using Logitar.Kraken.Core.Validators;

namespace Logitar.Kraken.Core.Sessions.Validators;

internal class RenewSessionValidator : AbstractValidator<RenewSessionPayload>
{
  public RenewSessionValidator()
  {
    RuleFor(x => x.RefreshToken).NotEmpty();

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeValidator());
  }
}
