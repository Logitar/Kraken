using Microsoft.Extensions.Configuration;

namespace Logitar.Kraken.Infrastructure.Settings;

internal record EncryptionSettings
{
  public string Key { get; set; } = string.Empty;

  public static EncryptionSettings Initialize(IConfiguration configuration)
  {
    EncryptionSettings settings = configuration.GetSection("Encryption").Get<EncryptionSettings>() ?? new();

    settings.Key = EnvironmentHelper.GetString("ENCRYPTION_KEY", settings.Key);

    return settings;
  }
}
