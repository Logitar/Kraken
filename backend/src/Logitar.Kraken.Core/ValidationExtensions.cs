using FluentValidation;
using Logitar.Kraken.Contracts.Settings;
using Logitar.Kraken.Core.Validators;

namespace Logitar.Kraken.Core;

public static class ValidationExtensions
{
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

  public static IRuleBuilderOptions<T, string> UniqueName<T>(this IRuleBuilder<T, string> ruleBuilder, IUniqueNameSettings settings)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Core.UniqueName.MaximumLength).SetValidator(new AllowedCharactersValidator<T>(settings.AllowedCharacters));
  }

  public static IRuleBuilderOptions<T, string> Url<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Core.Url.MaximumLength).SetValidator(new UrlValidator<T>());
  }
}
