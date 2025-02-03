﻿using FluentValidation;
using Logitar.Kraken.Contracts.Fields;

namespace Logitar.Kraken.Core.Fields.Validators;

public class RichTextPropertiesValidator : AbstractValidator<IRichTextProperties>
{
  public RichTextPropertiesValidator()
  {
    RuleFor(x => x.ContentType).ContentType();

    When(x => x.MinimumLength.HasValue, () => RuleFor(x => x.MinimumLength!.Value).GreaterThan(0));
    When(x => x.MaximumLength.HasValue, () => RuleFor(x => x.MaximumLength!.Value).GreaterThan(0));
    When(x => x.MinimumLength.HasValue && x.MaximumLength.HasValue, () =>
    {
      RuleFor(x => x.MinimumLength!.Value).LessThanOrEqualTo(x => x.MaximumLength!.Value);
      RuleFor(x => x.MaximumLength!.Value).GreaterThanOrEqualTo(x => x.MinimumLength!.Value);
    });
  }
}
