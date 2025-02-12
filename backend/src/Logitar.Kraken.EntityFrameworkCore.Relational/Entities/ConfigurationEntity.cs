namespace Logitar.Kraken.EntityFrameworkCore.Relational.Entities;

public sealed class ConfigurationEntity
{
  public int ConfigurationId { get; private set; }

  public string Key { get; private set; } = string.Empty;
  public string Value { get; private set; } = string.Empty;

  public long Version { get; private set; }

  public string? CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }

  public string? UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  private ConfigurationEntity()
  {
  }

  public override bool Equals(object? obj) => obj is ConfigurationEntity configuration && configuration.ConfigurationId == ConfigurationId;
  public override int GetHashCode() => ConfigurationId.GetHashCode();
  public override string ToString() => $"{Key}={Value} (ConfigurationId={ConfigurationId})";
}
