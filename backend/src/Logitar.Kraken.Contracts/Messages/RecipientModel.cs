using Logitar.Kraken.Contracts.Users;

namespace Logitar.Kraken.Contracts.Messages;

public record RecipientModel
{
  public RecipientType Type { get; set; }

  public string? Address { get; set; }
  public string? DisplayName { get; set; }

  public string? PhoneNumber { get; set; }

  public UserModel? User { get; set; }
}
