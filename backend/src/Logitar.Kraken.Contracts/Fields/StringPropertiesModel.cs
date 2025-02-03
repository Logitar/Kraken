namespace Logitar.Kraken.Contracts.Fields;

public record StringPropertiesModel : IStringProperties
{
  public int? MinimumLength { get; set; }
  public int? MaximumLength { get; set; }
  public string? Pattern { get; set; }

  public StringPropertiesModel()
  {
  }

  public StringPropertiesModel(IStringProperties @string)
  {
    MinimumLength = @string.MinimumLength;
    MaximumLength = @string.MaximumLength;
    Pattern = @string.Pattern;
  }
}
