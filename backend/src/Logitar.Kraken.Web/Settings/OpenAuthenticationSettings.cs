namespace Logitar.Kraken.Web.Settings;

public record OpenAuthenticationSettings
{
  public AccessTokenSettings AccessToken { get; set; } = new();

  public static OpenAuthenticationSettings Initialize(IConfiguration configuration)
  {
    OpenAuthenticationSettings settings = configuration.GetSection("OpenAuthentication").Get<OpenAuthenticationSettings>() ?? new();

    settings.AccessToken.ApplyEnvironment();

    return settings;
  }
}
