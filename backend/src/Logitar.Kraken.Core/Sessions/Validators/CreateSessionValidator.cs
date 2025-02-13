using FluentValidation;
using Logitar.Kraken.Contracts.Sessions;
using Logitar.Kraken.Core.Validators;

namespace Logitar.Kraken.Core.Sessions.Validators;

internal class CreateSessionValidator : AbstractValidator<CreateSessionPayload>
{
  public CreateSessionValidator()
  {
    RuleFor(x => x.User).NotEmpty();

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeValidator());
  }
}
