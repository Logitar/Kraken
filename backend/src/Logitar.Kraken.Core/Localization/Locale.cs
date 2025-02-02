using FluentValidation;

namespace Logitar.Kraken.Core.Localization;

public record Locale
{
  public const int MaximumLength = 16;

  public CultureInfo Culture { get; }
  public string Code { get; }

  public Locale(string code)
  {
    Code = code.Trim();
    new Validator().ValidateAndThrow(this);

    Culture = CultureInfo.GetCultureInfo(Code);
  }
  public Locale(CultureInfo culture)
  {
    Code = culture.Name;
    new Validator().ValidateAndThrow(this);

    Culture = culture;
  }

  public static Locale? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override string ToString() => $"{Culture.DisplayName} ({Culture.Name})";

  private class Validator : AbstractValidator<Locale>
  {
    public Validator()
    {
      RuleFor(x => x.Code).Locale();
    }
  }
}
