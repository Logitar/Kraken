namespace Logitar.Kraken.Contracts.Settings;

public record UniqueNameSettingsModel : IUniqueNameSettings
{
  public string? AllowedCharacters { get; set; } = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

  public UniqueNameSettingsModel(string? allowedCharacters = null)
  {
    AllowedCharacters = allowedCharacters;
  }

  public UniqueNameSettingsModel(IUniqueNameSettings uniqueName) : this(uniqueName.AllowedCharacters)
  {
  }
}
