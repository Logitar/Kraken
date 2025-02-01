using FluentValidation;
using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Core.Validators;

namespace Logitar.Kraken.Core;

public static class ValidationExtensions
{
  public static IRuleBuilderOptions<T, string> AllowedCharacters<T>(this IRuleBuilder<T, string> ruleBuilder, string? allowedCharacters)
  {
    return ruleBuilder.SetValidator(new AllowedCharactersValidator<T>(allowedCharacters));
  }

  public static IRuleBuilderOptions<T, string> CustomIdentifier<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Core.CustomIdentifier.MaximumLength);
  }

  public static IRuleBuilderOptions<T, string> Description<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Core.Description.MaximumLength);
  }

  public static IRuleBuilderOptions<T, string> DisplayName<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Core.DisplayName.MaximumLength);
  }

  public static IRuleBuilderOptions<T, DateTime> Future<T>(this IRuleBuilder<T, DateTime> ruleBuilder, DateTime? moment = null)
  {
    return ruleBuilder.SetValidator(new FutureValidator<T>(moment));
  }

  public static IRuleBuilderOptions<T, string> Gender<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Users.Gender.MaximumLength);
  }

  public static IRuleBuilderOptions<T, string> Identifier<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Core.Identifier.MaximumLength).SetValidator(new IdentifierValidator<T>());
  }

  public static IRuleBuilderOptions<T, string> JwtSecret<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MinimumLength(Tokens.JwtSecret.MinimumLength).MaximumLength(Tokens.JwtSecret.MaximumLength);
  }

  public static IRuleBuilderOptions<T, string> Locale<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Localization.Locale.MaximumLength).SetValidator(new LocaleValidator<T>());
  }

  public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder, IPasswordSettings passwordSettings)
  {
    IRuleBuilderOptions<T, string> options = ruleBuilder.NotEmpty();
    if (passwordSettings.RequiredLength > 0)
    {
      options = options.MinimumLength(passwordSettings.RequiredLength)
        .WithErrorCode("PasswordTooShort")
        .WithMessage($"Passwords must be at least {passwordSettings.RequiredLength} characters.");
    }
    if (passwordSettings.RequiredUniqueChars > 0)
    {
      options = options.Must(x => x.GroupBy(c => c).Count() >= passwordSettings.RequiredUniqueChars)
        .WithErrorCode("PasswordRequiresUniqueChars")
        .WithMessage($"Passwords must use at least {passwordSettings.RequiredUniqueChars} different characters.");
    }
    if (passwordSettings.RequireNonAlphanumeric)
    {
      options = options.Must(x => x.Any(c => !char.IsLetterOrDigit(c)))
        .WithErrorCode("PasswordRequiresNonAlphanumeric")
        .WithMessage("Passwords must have at least one non alphanumeric character.");
    }
    if (passwordSettings.RequireLowercase)
    {
      options = options.Must(x => x.Any(char.IsLower))
        .WithErrorCode("PasswordRequiresLower")
        .WithMessage("Passwords must have at least one lowercase ('a'-'z').");
    }
    if (passwordSettings.RequireUppercase)
    {
      options = options.Must(x => x.Any(char.IsUpper))
        .WithErrorCode("PasswordRequiresUpper")
        .WithMessage("Passwords must have at least one uppercase ('A'-'Z').");
    }
    if (passwordSettings.RequireDigit)
    {
      options = options.Must(x => x.Any(char.IsDigit))
        .WithErrorCode("PasswordRequiresDigit")
        .WithMessage("Passwords must have at least one digit ('0'-'9').");
    }
    return options;
  }

  public static IRuleBuilderOptions<T, DateTime> Past<T>(this IRuleBuilder<T, DateTime> ruleBuilder, DateTime? moment = null)
  {
    return ruleBuilder.SetValidator(new PastValidator<T>(moment));
  }

  public static IRuleBuilderOptions<T, string> PersonName<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Users.PersonName.MaximumLength);
  }

  public static IRuleBuilderOptions<T, string> Slug<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Realms.Slug.MaximumLength).SetValidator(new SlugValidator<T>());
  }

  public static IRuleBuilderOptions<T, string> TimeZone<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Localization.TimeZone.MaximumLength).SetValidator(new TimeZoneValidator<T>());
  }


  public static IRuleBuilderOptions<T, string> UniqueName<T>(this IRuleBuilder<T, string> ruleBuilder, IUniqueNameSettings settings)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Core.UniqueName.MaximumLength).SetValidator(new AllowedCharactersValidator<T>(settings.AllowedCharacters));
  }

  public static IRuleBuilderOptions<T, string> Url<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Core.Url.MaximumLength).SetValidator(new UrlValidator<T>());
  }
}
