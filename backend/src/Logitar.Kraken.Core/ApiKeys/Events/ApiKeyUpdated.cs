using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.ApiKeys.Events;

public record ApiKeyUpdated : DomainEvent, INotification
{
  public DisplayName? Name { get; set; }
  public Change<Description>? Description { get; set; }
  public DateTime? ExpiresOn { get; set; }

  public Dictionary<Identifier, string?> CustomAttributes { get; set; } = [];

  [JsonIgnore]
  public bool HasChanges => Name != null || Description != null || ExpiresOn != null || CustomAttributes.Count > 0;
}
