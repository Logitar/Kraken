using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Users.Events;

public record UserUpdated : DomainEvent, INotification
{
  public Dictionary<Identifier, string?> CustomAttributes { get; set; } = [];

  [JsonIgnore]
  public bool HasChanges => CustomAttributes.Count > 0;
}
