using Logitar.Kraken.Infrastructure;

namespace Logitar.Kraken.Web.Settings;

public record AccessTokenSettings
{
  public int Lifetime { get; set; } = 300;
  public string Type { get; set; } = "at+jwt";

  public void ApplyEnvironment()
  {
    Lifetime = EnvironmentHelper.GetInt32("OPEN_AUTHENTICATION_ACCESS_TOKEN_LIFETIME", Lifetime);
    Type = EnvironmentHelper.GetString("OPEN_AUTHENTICATION_ACCESS_TOKEN_TYPE", Type);
  }
}
