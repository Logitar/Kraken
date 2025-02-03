﻿using FluentValidation;
using Logitar.Kraken.Contracts.Fields;

namespace Logitar.Kraken.Core.Fields.Validators;

public class NumberPropertiesValidator : AbstractValidator<INumberProperties>
{
  public NumberPropertiesValidator()
  {
    When(x => x.MinimumValue.HasValue && x.MaximumValue.HasValue, () =>
    {
      RuleFor(x => x.MinimumValue!.Value).LessThanOrEqualTo(x => x.MaximumValue!.Value);
      RuleFor(x => x.MaximumValue!.Value).GreaterThanOrEqualTo(x => x.MinimumValue!.Value);
    });

    When(x => x.Step.HasValue, () => RuleFor(x => x.Step!.Value).GreaterThan(0.0));

    When(x => x.MaximumValue.HasValue && x.Step.HasValue, () =>
    {
      RuleFor(x => x.Step!.Value).LessThan(x => x.MaximumValue!.Value);
      RuleFor(x => x.MaximumValue!.Value).GreaterThan(x => x.Step!.Value);
    });
  }
}
