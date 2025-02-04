namespace Logitar.Kraken.Web.Settings;

public record CorsSettings
{
  public const string SectionKey = "Cors";

  public bool AllowAnyOrigin { get; set; }
  public string[] AllowedOrigins { get; set; } = [];

  public bool AllowAnyMethod { get; set; }
  public string[] AllowedMethods { get; set; } = [];

  public bool AllowAnyHeader { get; set; }
  public string[] AllowedHeaders { get; set; } = [];

  public bool AllowCredentials { get; set; }
}
