using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Roles.Events;

public record RoleUpdated : DomainEvent, INotification
{
  public UniqueName? UniqueName { get; set; }
  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }

  public Dictionary<Identifier, string?> CustomAttributes { get; set; } = [];

  [JsonIgnore]
  public bool HasChanges => UniqueName != null || DisplayName != null || Description != null || CustomAttributes.Count > 0;
}
