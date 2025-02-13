﻿using FluentValidation;
using Logitar.Kraken.Contracts.Tokens;
using Logitar.Kraken.Core.Users.Validators;

namespace Logitar.Kraken.Core.Tokens.Validators;

internal class CreateTokenValidator : AbstractValidator<CreateTokenPayload>
{
  public CreateTokenValidator()
  {
    When(x => x.LifetimeSeconds.HasValue, () => RuleFor(x => x.LifetimeSeconds!.Value).GreaterThan(0));
    When(x => !string.IsNullOrWhiteSpace(x.Secret), () => RuleFor(x => x.Secret!).Secret());

    When(x => x.Email != null, () => RuleFor(x => x.Email!).SetValidator(new EmailValidator()));
    RuleForEach(x => x.Claims).SetValidator(new ClaimValidator());
  }
}
