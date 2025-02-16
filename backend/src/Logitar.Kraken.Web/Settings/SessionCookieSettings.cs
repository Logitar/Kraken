using Logitar.Kraken.Infrastructure;

namespace Logitar.Kraken.Web.Settings;

public record SessionCookieSettings
{
  public SameSiteMode SameSite { get; set; } = SameSiteMode.Strict;

  public void ApplyEnvironment()
  {
    SameSite = EnvironmentHelper.GetEnum("COOKIES_SESSION_SAME_SITE", SameSite);
  }
}
