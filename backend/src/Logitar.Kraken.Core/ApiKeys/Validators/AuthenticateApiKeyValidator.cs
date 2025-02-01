using FluentValidation;
using Logitar.Kraken.Contracts.ApiKeys;

namespace Logitar.Kraken.Core.ApiKeys.Validators;

internal class AuthenticateApiKeyValidator : AbstractValidator<AuthenticateApiKeyPayload>
{
  public AuthenticateApiKeyValidator()
  {
    RuleFor(x => x.XApiKey).NotEmpty();
  }
}
