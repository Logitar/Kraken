using Logitar.Kraken.Infrastructure;

namespace Logitar.Kraken.Web.Constants;

public static class Schemes
{
  public const string ApiKey = "ApiKey";
  public const string Basic = "Basic";
  public const string Bearer = "Bearer";
  public const string Session = "Session";

  public static string[] GetEnabled(IConfiguration configuration)
  {
    List<string> schemes = new(capacity: 4)
    {
      ApiKey,
      Bearer,
      Session
    };

    bool isBasicAuthenticationEnabled = EnvironmentHelper.GetBoolean("BASIC_AUTHENTICATION_ENABLED", configuration.GetValue<bool>("EnableBasicAuthentication"));
    if (isBasicAuthenticationEnabled)
    {
      schemes.Add(Basic);
    }

    return [.. schemes];
  }
}
