using FluentValidation;
using Logitar.Kraken.Contracts.Users;

namespace Logitar.Kraken.Core.Users.Validators;

internal class EmailValidator : AbstractValidator<IEmail>
{
  public EmailValidator()
  {
    RuleFor(x => x.Address).NotEmpty().MaximumLength(Email.MaximumLength).EmailAddress();
  }
}
