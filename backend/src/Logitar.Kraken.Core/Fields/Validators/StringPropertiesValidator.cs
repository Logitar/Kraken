using FluentValidation;
using Logitar.Kraken.Contracts.Fields;

namespace Logitar.Kraken.Core.Fields.Validators;

internal class StringPropertiesValidator : AbstractValidator<IStringProperties>
{
  public StringPropertiesValidator()
  {
    When(x => x.MinimumLength.HasValue, () => RuleFor(x => x.MinimumLength!.Value).GreaterThan(0));
    When(x => x.MaximumLength.HasValue, () => RuleFor(x => x.MaximumLength!.Value).GreaterThan(0));
    When(x => x.MinimumLength.HasValue && x.MaximumLength.HasValue, () =>
    {
      RuleFor(x => x.MinimumLength!.Value).LessThanOrEqualTo(x => x.MaximumLength!.Value);
      RuleFor(x => x.MaximumLength!.Value).GreaterThanOrEqualTo(x => x.MinimumLength!.Value);
    });

    When(x => x.Pattern != null, () => RuleFor(x => x.Pattern!).NotEmpty());
  }
}
