using FluentValidation;
using Logitar.Kraken.Contracts.Settings;

namespace Logitar.Kraken.Core;

public record UniqueName
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }

  public UniqueName(IUniqueNameSettings settings, string value)
  {
    Value = value.Trim();
    new Validator(settings).ValidateAndThrow(this);
  }

  public static UniqueName? TryCreate(IUniqueNameSettings settings, string? value) => string.IsNullOrWhiteSpace(value) ? null : new(settings, value);

  public override string ToString() => Value;

  private class Validator : AbstractValidator<UniqueName>
  {
    public Validator(IUniqueNameSettings settings)
    {
      RuleFor(x => x.Value).UniqueName(settings);
    }
  }
}
