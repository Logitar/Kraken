using FluentValidation;
using Logitar.Security.Cryptography;

namespace Logitar.Kraken.Core.Tokens;

public record Secret // TODO(fpion): should be encrypted!
{
  public const int MinimumLength = 256 / 8;
  public const int MaximumLength = 512 / 8;

  public string Value { get; }

  public Secret(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public static Secret CreateOrGenerate(string? value) => string.IsNullOrWhiteSpace(value) ? Generate() : new(value);
  public static Secret Generate() => Generate(MinimumLength);
  public static Secret Generate(int length) => new(RandomStringGenerator.GetString(length));

  public override string ToString() => Value;

  private class Validator : AbstractValidator<Secret>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Secret();
    }
  }
}
