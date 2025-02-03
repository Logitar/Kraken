using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Dictionaries.Events;

public record DictionaryDeleted : DomainEvent, IDeleteEvent, INotification;
