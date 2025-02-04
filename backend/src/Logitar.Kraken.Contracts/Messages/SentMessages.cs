namespace Logitar.Kraken.Contracts.Messages;

public record SentMessages
{
  public List<Guid> Ids { get; set; } = [];

  public SentMessages(IEnumerable<Guid>? ids = null)
  {
    if (ids != null)
    {
      Ids.AddRange(ids);
    }
  }
}
