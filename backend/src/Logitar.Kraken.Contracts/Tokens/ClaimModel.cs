namespace Logitar.Kraken.Contracts.Tokens;

public record ClaimModel
{
  public string Name { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;
  public string? Type { get; set; }

  public ClaimModel()
  {
  }

  public ClaimModel(string name, string value, string? type = null)
  {
    Name = name;
    Value = value;
    Type = type;
  }
}
