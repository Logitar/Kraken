using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Templates.Events;

public record TemplateUpdated : DomainEvent, INotification
{
  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }

  public Subject? Subject { get; set; }
  public TemplateContent? Content { get; set; }

  [JsonIgnore]
  public bool HasChanges => DisplayName != null || Description != null || Subject != null || Content != null;
}
