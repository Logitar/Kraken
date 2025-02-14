namespace Logitar.Kraken.Settings;

internal record DefaultSettings
{
  public string Locale { get; set; } = "en";
  public string UniqueName { get; set; } = "admin";
  public string Password { get; set; } = "P@s$W0rD";

  public static DefaultSettings Initialize(IConfiguration configuration)
  {
    DefaultSettings settings = configuration.GetSection("Default").Get<DefaultSettings>() ?? new();

    string? locale = Environment.GetEnvironmentVariable("DEFAULT_LOCALE");
    if (!string.IsNullOrWhiteSpace(locale))
    {
      settings.Locale = locale.Trim();
    }

    string? uniqueName = Environment.GetEnvironmentVariable("DEFAULT_UNIQUE_NAME");
    if (!string.IsNullOrWhiteSpace(uniqueName))
    {
      settings.UniqueName = uniqueName.Trim();
    }

    string? password = Environment.GetEnvironmentVariable("DEFAULT_PASSWORD");
    if (!string.IsNullOrWhiteSpace(password))
    {
      settings.Password = password.Trim();
    }

    return settings;
  }
}
