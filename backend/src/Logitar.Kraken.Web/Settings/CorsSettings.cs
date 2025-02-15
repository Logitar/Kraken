using Logitar.Kraken.Infrastructure;

namespace Logitar.Kraken.Web.Settings;

public record CorsSettings
{
  public bool AllowAnyOrigin { get; set; }
  public string[] AllowedOrigins { get; set; } = [];

  public bool AllowAnyMethod { get; set; }
  public string[] AllowedMethods { get; set; } = [];

  public bool AllowAnyHeader { get; set; }
  public string[] AllowedHeaders { get; set; } = [];

  public bool AllowCredentials { get; set; }

  public static CorsSettings Initialize(IConfiguration configuration)
  {
    CorsSettings settings = configuration.GetSection("Cors").Get<CorsSettings>() ?? new();

    settings.AllowAnyOrigin = EnvironmentHelper.GetBoolean("CORS_ALLOW_ANY_ORIGIN", settings.AllowAnyOrigin);
    settings.AllowedOrigins = EnvironmentHelper.GetStrings("CORS_ALLOWED_ORIGINS", settings.AllowedOrigins);
    settings.AllowAnyMethod = EnvironmentHelper.GetBoolean("CORS_ALLOW_ANY_METHOD", settings.AllowAnyMethod);
    settings.AllowedMethods = EnvironmentHelper.GetStrings("CORS_ALLOWED_METHODS", settings.AllowedMethods);
    settings.AllowAnyHeader = EnvironmentHelper.GetBoolean("CORS_ALLOW_ANY_HEADER", settings.AllowAnyHeader);
    settings.AllowedHeaders = EnvironmentHelper.GetStrings("CORS_ALLOWED_HEADERS", settings.AllowedHeaders);
    settings.AllowCredentials = EnvironmentHelper.GetBoolean("CORS_ALLOW_CREDENTIALS", settings.AllowCredentials);

    return settings;
  }
}
