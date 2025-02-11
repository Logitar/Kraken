namespace Logitar.Kraken.Settings;

internal record DefaultSettings
{
  public string Locale { get; set; } = "en";
  public string UniqueName { get; set; } = "admin";
  public string Password { get; set; } = "P@s$W0rD";

  public static DefaultSettings Initialize(IConfiguration configuration)
  {
    DefaultSettings settings = configuration.GetSection("Default").Get<DefaultSettings>() ?? new();
    settings.Locale = EnvironmentHelper.GetVariable("DEFAULT_LOCALE", settings.Locale);
    settings.UniqueName = EnvironmentHelper.GetVariable("DEFAULT_UNIQUE_NAME", settings.UniqueName);
    settings.Password = EnvironmentHelper.GetVariable("DEFAULT_PASSWORD", settings.Password);
    return settings;
  }
}
