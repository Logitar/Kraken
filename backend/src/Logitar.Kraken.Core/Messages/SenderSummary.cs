using Logitar.Kraken.Contracts.Senders;
using Logitar.Kraken.Core.Senders;
using Logitar.Kraken.Core.Users;

namespace Logitar.Kraken.Core.Messages;

public record SenderSummary
{
  public SenderId Id { get; }
  public bool IsDefault { get; }
  public Email? Email { get; }
  public Phone? Phone { get; }
  public DisplayName? DisplayName { get; }
  public SenderProvider Provider { get; }

  [JsonConstructor]
  public SenderSummary(SenderId id, bool isDefault, Email? email, Phone? phone, DisplayName? displayName, SenderProvider provider)
  {
    Id = id;
    IsDefault = isDefault;
    Email = email;
    Phone = phone;
    DisplayName = displayName;
    Provider = provider;
  }

  public SenderSummary(SenderId id, bool isDefault, Email email, DisplayName? displayName, SenderProvider provider)
    : this(id, isDefault, email, phone: null, displayName, provider)
  {
  }

  public SenderSummary(Sender sender) : this(sender.Id, sender.IsDefault, sender.Email, sender.Phone, sender.DisplayName, sender.Provider)
  {
  }
}
