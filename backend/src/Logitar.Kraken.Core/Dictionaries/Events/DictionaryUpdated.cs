using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Dictionaries.Events;

public record DictionaryUpdated : DomainEvent, INotification
{
  public Dictionary<Identifier, string?> Entries { get; set; } = [];

  [JsonIgnore]
  public bool HasChanges => Entries.Count > 0;
}
