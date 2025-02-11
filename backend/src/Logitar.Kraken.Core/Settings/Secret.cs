using FluentValidation;

namespace Logitar.Kraken.Core.Settings;

public partial record Secret
{
  public string Value { get; }

  public Secret(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public static Secret? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override string ToString() => Value;

  private class Validator : AbstractValidator<Secret>
  {
    public Validator()
    {
      RuleFor(x => x.Value).NotEmpty();
    }
  }
}
