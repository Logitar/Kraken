using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Fields.Events;

public record FieldTypeUpdated : DomainEvent, INotification
{
  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }

  [JsonIgnore]
  public bool HasChanges => DisplayName != null || Description != null;
}
