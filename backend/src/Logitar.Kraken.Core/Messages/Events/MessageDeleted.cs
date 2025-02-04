using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Messages.Events;

public record MessageDeleted : DomainEvent, IDeleteEvent, INotification;
