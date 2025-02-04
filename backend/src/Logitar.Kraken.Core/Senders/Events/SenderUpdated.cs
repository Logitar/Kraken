using Logitar.EventSourcing;
using Logitar.Kraken.Core.Users;
using MediatR;

namespace Logitar.Kraken.Core.Senders.Events;

public record SenderUpdated : DomainEvent, INotification
{
  public Email? Email { get; set; }
  public Phone? Phone { get; set; }
  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }

  [JsonIgnore]
  public bool HasChanges => Email != null || Phone != null || DisplayName != null || Description != null;
}
