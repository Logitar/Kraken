using FluentValidation;
using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Core.Settings.Validators;

namespace Logitar.Kraken.Core.Settings;

public record PasswordSettings : IPasswordSettings
{
  public int RequiredLength { get; }
  public int RequiredUniqueChars { get; }
  public bool RequireNonAlphanumeric { get; }
  public bool RequireLowercase { get; }
  public bool RequireUppercase { get; }
  public bool RequireDigit { get; }
  public string HashingStrategy { get; }

  public PasswordSettings()
    : this(requiredLength: 8, requiredUniqueChars: 8, requireNonAlphanumeric: true, requireLowercase: true, requireUppercase: true, requireDigit: true, hashingStrategy: "PBKDF2")
  {
  }

  public PasswordSettings(IPasswordSettings settings)
    : this(settings.RequiredLength, settings.RequiredUniqueChars, settings.RequireNonAlphanumeric, settings.RequireLowercase, settings.RequireUppercase, settings.RequireDigit, settings.HashingStrategy)
  {
  }

  [JsonConstructor]
  public PasswordSettings(int requiredLength, int requiredUniqueChars, bool requireNonAlphanumeric, bool requireLowercase, bool requireUppercase, bool requireDigit, string hashingStrategy)
  {
    RequiredLength = requiredLength;
    RequiredUniqueChars = requiredUniqueChars;
    RequireNonAlphanumeric = requireNonAlphanumeric;
    RequireLowercase = requireLowercase;
    RequireUppercase = requireUppercase;
    RequireDigit = requireDigit;
    HashingStrategy = hashingStrategy;
    new PasswordSettingsValidator().ValidateAndThrow(this);
  }
}
