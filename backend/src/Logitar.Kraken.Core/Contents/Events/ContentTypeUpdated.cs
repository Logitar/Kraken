using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Contents.Events;

public record ContentTypeUpdated : DomainEvent, INotification
{
  public bool? IsInvariant { get; set; }

  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }

  [JsonIgnore]
  public bool HasChanges => IsInvariant != null || DisplayName != null || Description != null;
}
