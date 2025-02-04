namespace Logitar.Kraken.Contracts.Messages;

public record SentMessages
{
  public List<Guid> Ids { get; set; } = [];
}
