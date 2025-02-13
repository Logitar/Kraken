using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Kraken.Core.Sessions.Events;

public record SessionDeleted : DomainEvent, IDeleteEvent, INotification;
