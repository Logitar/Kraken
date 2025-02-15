namespace Logitar.Kraken.Web.Settings;

public record CookiesSettings
{
  public RefreshTokenCookieSettings RefreshToken { get; set; } = new();
  public SessionCookieSettings Session { get; set; } = new();

  public static CookiesSettings Initialize(IConfiguration configuration)
  {
    CookiesSettings settings = configuration.GetSection("Cookies").Get<CookiesSettings>() ?? new();

    settings.RefreshToken.ApplyEnvironment();
    settings.Session.ApplyEnvironment();

    return settings;
  }
}
