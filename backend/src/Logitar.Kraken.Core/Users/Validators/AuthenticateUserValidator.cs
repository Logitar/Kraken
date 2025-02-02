using FluentValidation;
using Logitar.Kraken.Contracts.Users;

namespace Logitar.Kraken.Core.Users.Validators;

internal class AuthenticateUserValidator : AbstractValidator<AuthenticateUserPayload>
{
  public AuthenticateUserValidator()
  {
    RuleFor(x => x.UniqueName).NotEmpty();
    RuleFor(x => x.Password).NotEmpty();
  }
}
