using FluentValidation;
using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Core;

namespace Logitar.Kraken.Infrastructure.Passwords;

internal record PasswordInput
{
  public string Password { get; }

  public PasswordInput(IPasswordSettings settings, string password)
  {
    Password = password;
    new Validator(settings).ValidateAndThrow(this);
  }

  private class Validator : AbstractValidator<PasswordInput>
  {
    public Validator(IPasswordSettings settings)
    {
      RuleFor(x => x.Password).Password(settings);
    }
  }
}
