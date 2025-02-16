using Logitar.Kraken.Infrastructure;

namespace Logitar.Kraken.Web.Settings;

public record RefreshTokenCookieSettings
{
  public bool HttpOnly { get; set; } = true;
  public TimeSpan? MaxAge { get; set; } = TimeSpan.FromDays(7);
  public SameSiteMode SameSite { get; set; } = SameSiteMode.Strict;
  public bool Secure { get; set; } = true;

  public void ApplyEnvironment()
  {
    HttpOnly = EnvironmentHelper.GetBoolean("COOKIES_REFRESH_TOKEN_HTTP_ONLY", HttpOnly);

    string? maxAgeValue = Environment.GetEnvironmentVariable("COOKIES_REFRESH_TOKEN_MAX_AGE");
    if (!string.IsNullOrWhiteSpace(maxAgeValue) && TimeSpan.TryParse(maxAgeValue.Trim(), out TimeSpan maxAge))
    {
      MaxAge = maxAge;
    }

    SameSite = EnvironmentHelper.GetEnum("COOKIES_REFRESH_TOKEN_SAME_SITE", SameSite);
    Secure = EnvironmentHelper.GetBoolean("COOKIES_REFRESH_TOKEN_SECURE", Secure);
  }
}
