using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Roles.Events;

public record RoleUpdated : DomainEvent, INotification
{
  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }

  public Dictionary<Identifier, string?> CustomAttributes { get; set; } = [];

  [JsonIgnore]
  public bool HasChanges => DisplayName != null || Description != null || CustomAttributes.Count > 0;
}
