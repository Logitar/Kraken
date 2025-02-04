namespace Logitar.Kraken.Contracts.Messages;

public record SendMessagePayload
{
  public string? Sender { get; set; }
  public string Template { get; set; } = string.Empty;

  public List<RecipientPayload> Recipients { get; set; } = [];

  public bool IgnoreUserLocale { get; set; }
  public string? Locale { get; set; }

  public List<Variable> Variables { get; set; } = [];

  public bool IsDemo { get; set; }
}
