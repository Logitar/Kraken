using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Passwords.Events;

public record OneTimePasswordUpdated : DomainEvent, INotification
{
  public Dictionary<Identifier, string?> CustomAttributes { get; set; } = [];

  [JsonIgnore]
  public bool HasChanges => CustomAttributes.Count > 0;
}
