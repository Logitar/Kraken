﻿using FluentValidation;
using Logitar.Kraken.Contracts.Configurations;
using Logitar.Kraken.Core.Settings.Validators;

namespace Logitar.Kraken.Core.Configurations.Validators;

internal class UpdateConfigurationValidator : AbstractValidator<UpdateConfigurationPayload>
{
  public UpdateConfigurationValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Secret), () => RuleFor(x => x.Secret!).Secret());

    When(x => x.UniqueNameSettings != null, () => RuleFor(x => x.UniqueNameSettings!).SetValidator(new UniqueNameSettingsValidator()));
    When(x => x.PasswordSettings != null, () => RuleFor(x => x.PasswordSettings!).SetValidator(new PasswordSettingsValidator()));
  }
}
