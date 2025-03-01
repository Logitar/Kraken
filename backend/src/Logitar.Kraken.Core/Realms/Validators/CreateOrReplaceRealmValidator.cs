﻿using FluentValidation;
using Logitar.Kraken.Contracts.Realms;
using Logitar.Kraken.Core.Settings.Validators;
using Logitar.Kraken.Core.Validators;

namespace Logitar.Kraken.Core.Realms.Validators;

internal class CreateOrReplaceRealmValidator : AbstractValidator<CreateOrReplaceRealmPayload>
{
  public CreateOrReplaceRealmValidator()
  {
    RuleFor(x => x.UniqueSlug).Slug();
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    When(x => !string.IsNullOrWhiteSpace(x.Secret), () => RuleFor(x => x.Secret!).Secret());
    When(x => !string.IsNullOrWhiteSpace(x.Url), () => RuleFor(x => x.Url!).Url());

    RuleFor(x => x.UniqueNameSettings).SetValidator(new UniqueNameSettingsValidator());
    RuleFor(x => x.PasswordSettings).SetValidator(new PasswordSettingsValidator());

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeValidator());
  }
}
