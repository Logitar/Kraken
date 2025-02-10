using Logitar.Kraken.Constants;

namespace Logitar.Kraken.Models.Index;

public record ApiVersion
{
  public static ApiVersion Current => new(Api.Title, Api.Version);

  public string Title { get; set; } = string.Empty;
  public string Version { get; set; } = string.Empty;

  public ApiVersion()
  {
  }

  public ApiVersion(string title, Version version) : this(title, version.ToString())
  {
  }

  public ApiVersion(string title, string version)
  {
    Title = title;
    Version = version;
  }
}
