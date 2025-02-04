using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Senders.Events;

public record SenderDeleted : DomainEvent, IDeleteEvent, INotification;
