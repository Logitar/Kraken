namespace Logitar.Kraken.Contracts.Contents;

public record UpdateContentTypePayload
{
  public bool? IsInvariant { get; set; }

  public string? UniqueName { get; set; }
  public ChangeModel<string>? DisplayName { get; set; }
  public ChangeModel<string>? Description { get; set; }
}
