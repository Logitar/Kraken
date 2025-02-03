namespace Logitar.Kraken.Contracts.Contents;

public class ContentTypeSummary
{
  public Guid Id { get; set; }
  public string UniqueName { get; set; } = string.Empty;

  public ContentTypeSummary()
  {
  }

  public ContentTypeSummary(Guid id, string uniqueName)
  {
    Id = id;
    UniqueName = uniqueName;
  }

  public override bool Equals(object? obj) => obj is ContentTypeSummary contentType && contentType.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{UniqueName} (Id={Id})";
}
