using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Sessions.Events;

public record SessionUpdated : DomainEvent, INotification
{
  public Dictionary<Identifier, string?> CustomAttributes { get; set; } = [];

  [JsonIgnore]
  public bool HasChanges => CustomAttributes.Count > 0;
}
